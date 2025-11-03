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
    private readonly IUserRepository userRepository;

    public CommentsController(
        ICommentRepository commentRepository,
        IPostRepository postRepository,
        IUserRepository userRepository)
    {
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    [HttpPost("/posts/{postId:int}/comments")]
    public async Task<ActionResult<CommentDTO>> AddComment(int postId, [FromBody] CreateCommentDTO request)
    {
        try
        {
            var user = await userRepository.GetSingleAsync(request.UserId);
            if (user == null)
            {
                return NotFound($"User with ID {request.UserId} not found");
            }
            
            var post = await postRepository.GetSingleAsync(postId);
            if (post == null)
            {
                return NotFound($"Post with ID {postId} not found");
            }

            Comment comment = new()
            {
                Body = request.Body,
                PostId = postId,
                UserId = request.UserId
            };

            Comment created = await commentRepository.AddAsync(comment);

            CommentDTO commentDTO = new()
            {
                Id = created.Id,
                Body = created.Body,
                PostId = created.PostId,
                UserId = created.UserId
            };

            return Created($"/comments/{created.Id}", commentDTO);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateComment(
        [FromRoute] int id,
        [FromBody] UpdateCommentDTO request
    )
    {
        try
        {
            Comment? existing = await commentRepository.GetSingleAsync(id);

            if (existing == null)
            {
                return NotFound();
            }

            existing.Body = request.Body ?? existing.Body;

            await commentRepository.UpdateAsync(existing);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPatch("/posts/{postId:int}/comments/{commentId:int}")]
    public async Task<IActionResult> UpdateCommentOfPost(
        [FromRoute] int postId,
        [FromRoute] int commentId,
        [FromBody] UpdateCommentDTO request
    )
    {
        try
        {
            var post = await postRepository.GetSingleAsync(postId);
            if (post == null)
            {
                return NotFound($"Post with ID {postId} not found");
            }

            Comment? existing = await commentRepository.GetSingleAsync(commentId);
            if (existing == null)
            {
                return NotFound($"Comment with ID {commentId} not found");
            }

            if (existing.PostId != postId)
            {
                return BadRequest($"Comment with ID {commentId} does not belong to post with ID {postId}");
            }

            existing.Body = request.Body ?? existing.Body;
            await commentRepository.UpdateAsync(existing);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDTO>> GetCommentById([FromRoute] int id)
    {
        try
        {
            Comment? comment = await commentRepository.GetSingleAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            CommentDTO commentDTO = new()
            {
                Id = comment.Id,
                Body = comment.Body,
                PostId = comment.PostId,
                UserId = comment.UserId
            };

            return Ok(commentDTO);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDTO>>> GetAllComments(
        [FromQuery] int? PostId = null,
        [FromQuery] int? UserId = null
    )
    {
        try
        {
            var comments = await commentRepository.GetManyAsync();

            if (PostId.HasValue)
            {
                comments = comments.Where(c => c.PostId == PostId.Value);
            }

            if (UserId.HasValue)
            {
                comments = comments.Where(c => c.UserId == UserId.Value);
            }

            var commentDTOs = comments.Select(c => new CommentDTO
            {
                Id = c.Id,
                Body = c.Body,
                PostId = c.PostId,
                UserId = c.UserId
            }).ToList();

            return Ok(commentDTOs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment([FromRoute] int id)
    {
        try
        {
            var existing = await commentRepository.GetSingleAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            await commentRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("/posts/{postId}/comments/{commentId}")]
    public async Task<IActionResult> DeleteCommentFromPost(
        [FromRoute] int postId,
        [FromRoute] int commentId)
    {
        try
        {
            var post = await postRepository.GetSingleAsync(postId);
            if (post == null)
            {
                return NotFound($"Post with ID {postId} not found");
            }

            Comment? comment = await commentRepository.GetSingleAsync(commentId);
            if (comment == null)
            {
                return NotFound($"Comment with ID {commentId} not found");
            }

            if (comment.PostId != postId)
            {
                return BadRequest($"Comment with ID {commentId} does not belong to post with ID {postId}");
            }

            await commentRepository.DeleteAsync(commentId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}