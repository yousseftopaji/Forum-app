using Microsoft.AspNetCore.Mvc;
using Entities;
using RepositoryContracts;
using DTOs.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;

    public CommentsController(
        ICommentRepository commentRepository,
        IPostRepository postRepository)
    {
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
    }

    [HttpPost]
    public async Task<IResult> AddComment([FromBody] CreateCommentDTO request)
    {
        // Verify post exists
        var post = await postRepository.GetSingleAsync(request.PostId);
        if (post == null)
        {
            return Results.BadRequest($"Post with ID {request.PostId} not found");
        }

        Comment comment = new()
        {
            Body = request.Body,
            PostId = request.PostId,
            UserId = request.UserId
        };

        Comment created = await commentRepository.AddAsync(comment);
        return Results.Created($"/comments/{created.Id}", created);
    }

    [HttpPatch("{id:int}")]
    public async Task<IResult> UpdateComment(
        [FromRoute] int id,
        [FromBody] UpdateCommentDTO request
    )
    {
        Comment? existing = await commentRepository.GetSingleAsync(id);

        if (existing == null)
        {
            return Results.NotFound();
        }

        existing.Body = request.Body ?? existing.Body;

        await commentRepository.UpdateAsync(existing);

        return Results.NoContent();
    }

    // Update a comment of a specific post
    [HttpPatch("/posts/{postId:int}/comments/{commentId:int}")]
    public async Task<IResult> UpdateCommentOfPost(
        [FromRoute] int postId,
        [FromRoute] int commentId,
        [FromBody] UpdateCommentDTO request
    )
    {
        // Verify post exists
        var post = await postRepository.GetSingleAsync(postId);
        if (post == null)
        {
            return Results.NotFound($"Post with ID {postId} not found");
        }

        Comment? existing = await commentRepository.GetSingleAsync(commentId);
        if (existing == null)
        {
            return Results.NotFound($"Comment with ID {commentId} not found");
        }

        // Verify the comment belongs to this post
        if (existing.PostId != postId)
        {
            return Results.BadRequest($"Comment with ID {commentId} does not belong to post with ID {postId}");
        }

        existing.Body = request.Body ?? existing.Body;
        await commentRepository.UpdateAsync(existing);

        return Results.NoContent();
    }

    [HttpGet("{id:int}")]
    public async Task<IResult> GetCommentById([FromRoute] int id)
    {
        Comment? comment = await commentRepository.GetSingleAsync(id);
        if (comment == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(comment);
    }

    [HttpGet]
    public IResult GetAllComments(
        [FromQuery] int? PostId = null,
        [FromQuery] int? UserId = null
    )
    {
        var comments = commentRepository.GetManyAsync();

        if (PostId.HasValue)
        {
            comments = comments.Where(c => c.PostId == PostId.Value);
        }

        if (UserId.HasValue)
        {
            comments = comments.Where(c => c.UserId == UserId.Value);
        }

        return Results.Ok(comments.ToList());
    }

    [HttpDelete("{id:int}")]
    public async Task<IResult> DeleteComment([FromRoute] int id)
    {
        await commentRepository.DeleteAsync(id);
        return Results.NoContent();
    }
    
    // Delete a comment from a specific post
    [HttpDelete("/posts/{postId}/comments/{commentId}")]
    public async Task<IResult> DeleteCommentFromPost(
        [FromRoute] int postId,
        [FromRoute] int commentId)
    {
        // Verify post exists
        var post = await postRepository.GetSingleAsync(postId);
        if (post == null)
        {
            return Results.NotFound($"Post with ID {postId} not found");
        }

        Comment? comment = await commentRepository.GetSingleAsync(commentId);
        if (comment == null)
        {
            return Results.NotFound($"Comment with ID {commentId} not found");
        }

        // Verify the comment belongs to this post
        if (comment.PostId != postId)
        {
            return Results.BadRequest($"Comment with ID {commentId} does not belong to post with ID {postId}");
        }

        await commentRepository.DeleteAsync(commentId);
        return Results.NoContent();
    }
}