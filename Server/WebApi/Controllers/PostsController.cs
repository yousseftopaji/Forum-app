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
            Id = created.Id,
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

        // Map to DTO
        PostDTO postDTO = new()
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId
        };
        return Results.Ok(postDTO);
    }

    [HttpGet]
    public async Task<IResult> GetPosts(
        [FromQuery] string? titleContains = null,
        [FromQuery] string? body = null,
        [FromQuery] string? authorUserId = null
    )
    {
        var postsToQuery = await postRepository.GetManyAsync();

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

        var postsList = postsToQuery.ToList();

        // Map to DTOs
        var postDTOs = postsList.Select(p => new PostDTO
        {
            Id = p.Id,
            Title = p.Title,
            Body = p.Body,
            UserId = p.UserId
        }).ToList();

        return Results.Ok(postDTOs);
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

        var commentsQuery = await commentRepository.GetManyAsync();
        var comments = commentsQuery
            .Where(c => c.PostId == postId)
            .ToList();

        // Map to DTOs
        var commentDTOs = comments.Select(c => new CommentDTO
        {
            Id = c.Id,
            Body = c.Body,
            PostId = c.PostId,
            UserId = c.UserId
        }).ToList();

        return Results.Ok(commentDTOs);
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

        // Map to DTO
        CommentDTO commentDTO = new()
        {
            Id = created.Id,
            Body = created.Body,
            PostId = created.PostId,
            UserId = created.UserId
        };

        return Results.Created($"/posts/{postId}/comments/{created.Id}", commentDTO);
    }

    [HttpDelete("{id:int}")]
    public async Task<IResult> DeletePost([FromRoute] int id)
    {
        await postRepository.DeleteAsync(id);
        return Results.NoContent();
    }
}