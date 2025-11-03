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


    [HttpPost("/users/{userId:int}/posts")]
    public async Task<ActionResult<PostDTO>> AddPost(
        [FromRoute] int userId,
        [FromBody] CreatePostDTO request)
    {
        try
        {
            var user = await userRepository.GetSingleAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found");
            }

            Post post = new()
            {
                Title = request.Title,
                Body = request.Body,
                UserId = userId
            };

            Post created = await postRepository.AddAsync(post);
            PostDTO postDTO = new()
            {
                Id = created.Id,
                Title = created.Title,
                Body = created.Body,
                UserId = created.UserId
            };

            return CreatedAtAction(nameof(GetPostById), new { id = created.Id }, postDTO);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdatePost(
        [FromRoute] int id,
        [FromBody] UpdatePostDTO request
    )
    {
        try
        {
            Post? existing = await postRepository.GetSingleAsync(id);

            if (existing == null)
            {
                return NotFound();
            }

            existing.Title = request.Title ?? existing.Title;
            existing.Body = request.Body ?? existing.Body;

            await postRepository.UpdateAsync(existing);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDTO>> GetPostById([FromRoute] int id)
    {
        try
        {
            Post? post = await postRepository.GetSingleAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            PostDTO postDTO = new()
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                UserId = post.UserId
            };

            return Ok(postDTO);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDTO>>> GetPosts(
        [FromQuery] string? titleContains = null,
        [FromQuery] string? body = null,
        [FromQuery] int? userId = null
    )
    {
        try
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

            if (userId.HasValue)
            {
                postsToQuery = postsToQuery.Where(p => p.UserId == userId.Value);
            }

            var postDTOs = postsToQuery.Select(p => new PostDTO
            {
                Id = p.Id,
                Title = p.Title,
                Body = p.Body,
                UserId = p.UserId
            }).ToList();

            return Ok(postDTOs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{postId:int}/comments")]
    public async Task<ActionResult<IEnumerable<CommentDTO>>> GetPostComments([FromRoute] int postId)
    {
        try
        {
            var post = await postRepository.GetSingleAsync(postId);
            if (post == null)
            {
                return NotFound($"Post with ID {postId} not found");
            }

            var comments = await commentRepository.GetManyAsync();
            var commentDTOs = comments
                .Where(c => c.PostId == postId)
                .Select(c => new CommentDTO
                {
                    Id = c.Id,
                    Body = c.Body,
                    PostId = c.PostId,
                    UserId = c.UserId
                })
                .ToList();

            return Ok(commentDTOs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePost([FromRoute] int id)
    {
        try
        {
            var existing = await postRepository.GetSingleAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            await postRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}