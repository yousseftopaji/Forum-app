using Microsoft.AspNetCore.Mvc;
using Entities;
using RepositoryContracts;
using DTOs.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;

    public PostsController(
        IPostRepository postRepository,
        ICommentRepository commentRepository,
        IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
    }

    // Create a post (standard route)
    [HttpPost]
    public async Task<ActionResult<PostDTO>> AddPost([FromBody] CreatePostDTO request)
    {
        // Verify user exists
        var user = await userRepository.GetSingleAsync(int.Parse(request.AuthorUserId));
        if (user == null)
        {
            return BadRequest($"User with ID {request.AuthorUserId} not found");
        }

        Post post = new()
        {
            Title = request.Title,
            Body = request.Body,
            UserId = int.Parse(request.AuthorUserId)
        };

        Post created = await postRepository.AddAsync(post);
        PostDTO postDTO = new()
        {
            Title = created.Title,
            Body = created.Body,
            UserId = created.UserId
        };

        return Created($"/posts/{created.Id}", postDTO);
    }

    [HttpPatch("{id:int}")]
    public async Task<IResult> UpdatePost(
        [FromRoute] int id,
        [FromBody] UpdatePostDTO request
    )
    {
        Post existing = await postRepository.GetSingleAsync(id);

        if (existing == null)
        {
            return Results.NotFound();
        }

        existing.Title = request.Title ?? existing.Title;
        existing.Body = request.Body ?? existing.Body;

        await postRepository.UpdateAsync(existing);

        return Results.NoContent();
    }

    [HttpGet("{id:int}")]
    public async Task<IResult> GetPostById([FromRoute] int id)
    {
        Post? post = await postRepository.GetSingleAsync(id);
        if (post == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(post);
    }

    [HttpGet]
    public IResult GetPosts(
        [FromQuery] string? titleContains = null,
        [FromQuery] string? body = null,
        [FromQuery] string? authorUserId = null
    )
    {
        var postsToQuery = postRepository.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(titleContains))
        {
            postsToQuery = postsToQuery.Where(
                p => p.Title.Contains(titleContains, StringComparison.OrdinalIgnoreCase)
            );
        }

        if (!string.IsNullOrWhiteSpace(body))
        {
            postsToQuery = postsToQuery.Where(
                p => p.Body.Contains(body, StringComparison.OrdinalIgnoreCase)
            );
        }

        if (!string.IsNullOrWhiteSpace(authorUserId) && int.TryParse(authorUserId, out int userId))
        {
            postsToQuery = postsToQuery.Where(p => p.UserId == userId);
        }

        return Results.Ok(postsToQuery.ToList());
    }

    // Get comments for a specific post
    [HttpGet("{postId:int}/comments")]
    public async Task<IResult> GetPostComments([FromRoute] int postId)
    {
        // Check if post exists
        var post = await postRepository.GetSingleAsync(postId);
        if (post == null)
        {
            return Results.NotFound($"Post with ID {postId} not found");
        }

        var comments = commentRepository.GetManyAsync()
            .Where(c => c.PostId == postId)
            .ToList();

        return Results.Ok(comments);
    }

    // Add a comment to a specific post
    [HttpPost("{postId:int}/comments")]
    public async Task<IResult> AddCommentToPost(
        [FromRoute] int postId,
        [FromBody] CreateCommentDTO request)
    {
        // Check if post exists
        var post = await postRepository.GetSingleAsync(postId);
        if (post == null)
        {
            return Results.NotFound($"Post with ID {postId} not found");
        }

        Comment comment = new()
        {
            Body = request.Body,
            PostId = postId,
            UserId = request.UserId
        };

        Comment created = await commentRepository.AddAsync(comment);
        return Results.Created($"/posts/{postId}/comments/{created.Id}", created);
    }

    [HttpDelete("{id:int}")]
    public async Task<IResult> DeletePost([FromRoute] int id)
    {
        await postRepository.DeleteAsync(id);
        return Results.NoContent();
    }
}