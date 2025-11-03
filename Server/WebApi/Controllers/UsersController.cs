using Microsoft.AspNetCore.Mvc;
using Entities;
using RepositoryContracts;
using DTOs.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;

    public UsersController(IUserRepository userRepository, IPostRepository postRepository)
    {
        this.userRepository = userRepository;
        this.postRepository = postRepository;
    }

    [HttpPost]
    public async Task<ActionResult<UserDTO>> AddUser([FromBody] CreateUserDTO request)
    {
        // Check if username already exists
        try
        {
            await VerifyUsernameIsUnique(request.Username);
            User user = new()
            {
                Username = request.Username,
                Password = request.Password
            };

            User created = await userRepository.AddAsync(user);
            //This is what the client should receive (userD)
            UserDTO userDTO = new()
            {
                Username = created.Username
            };
            return Created($"/users/{created.Id}", userDTO);
        }
        catch (ArgumentException ex)
        {
            return StatusCode(409, ex.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
    private async Task VerifyUsernameIsUnique(string username)
    {
        var users = await userRepository.GetManyAsync();
        var existingUser = users
            .FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (existingUser != null)
        {
            throw new ArgumentException("Username already exists");
        }
    }


    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateUser(
        [FromRoute] int id,
        [FromBody] UpdateUserDTO request
    )
    {
        try
        {
            User? existing = await userRepository.GetSingleAsync(id);

            if (existing == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(request.Username) &&
                !existing.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    await VerifyUsernameIsUnique(request.Username);
                    existing.Username = request.Username;
                }
                catch (ArgumentException ex)
                {
                    return Conflict(ex.Message);
                }
            }

            existing.Password = request.Password ?? existing.Password;

            await userRepository.UpdateAsync(existing);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDTO>> GetUserById([FromRoute] int id)
    {
        try
        {
            User? user = await userRepository.GetSingleAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            UserDTO userDTO = new()
            {
                Id = user.Id,
                Username = user.Username
            };

            return Ok(userDTO);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers([FromQuery] string? UsernameContains = null)
    {
        try
        {
            var users = await userRepository.GetManyAsync();

            if (!string.IsNullOrWhiteSpace(UsernameContains))
            {
                users = users.Where(u => u.Username.Contains(UsernameContains, StringComparison.OrdinalIgnoreCase));
            }

            var userDTOs = users.Select(u => new UserDTO
            {
                Id = u.Id,
                Username = u.Username
            }).ToList();

            return Ok(userDTOs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{userId:int}/posts")]
    public async Task<ActionResult<IEnumerable<PostDTO>>> GetUserPosts([FromRoute] int userId)
    {
        try
        {
            var user = await userRepository.GetSingleAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found");
            }

            var posts = await postRepository.GetManyAsync();
            var postDTOs = posts
                .Where(p => p.UserId == userId)
                .Select(p => new PostDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    Body = p.Body,
                    UserId = p.UserId
                })
                .ToList();

            return Ok(postDTOs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{userId:int}/posts/{postId:int}")]
    public async Task<ActionResult<PostDTO>> GetUserPost([FromRoute] int userId, [FromRoute] int postId)
    {
        try
        {
            var user = await userRepository.GetSingleAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found");
            }

            var posts = await postRepository.GetManyAsync();
            var post = posts.FirstOrDefault(p => p.Id == postId && p.UserId == userId);

            if (post == null)
            {
                return NotFound($"Post with ID {postId} not found for user {userId}");
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        try
        {
            var existing = await userRepository.GetSingleAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            await userRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}