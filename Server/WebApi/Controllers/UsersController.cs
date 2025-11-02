using Microsoft.AspNetCore.Mvc;
using Entities;
using RepositoryContracts;
using DTOs.Models;
using Microsoft.EntityFrameworkCore;

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
    }
    private async Task VerifyUsernameIsUnique(string username)
    {
        var users = await userRepository.GetManyAsync();
        var existingUser = await users
            .FirstOrDefaultAsync(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (existingUser != null)
        {
            throw new ArgumentException("Username already exists");
        }
    }


    [HttpPatch("{id:int}")]
    public async Task<IResult> UpdateUser(
        [FromRoute] int id,
        [FromBody] UpdateUserDTO request
    )
    {
        User? existing = await userRepository.GetSingleAsync(id);

        if (existing == null)
        {
            return Results.NotFound();
        }

        // Check if username is being changed and verify it's unique
        if (request.Username != null && !existing.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                await VerifyUsernameIsUnique(request.Username);
                existing.Username = request.Username;
            }
            catch (ArgumentException ex)
            {
                return Results.Conflict(ex.Message);
            }
        }

        existing.Password = request.Password ?? existing.Password;

        await userRepository.UpdateAsync(existing);

        return Results.NoContent();
    }

    [HttpGet("{id:int}")]
    public async Task<IResult> GetUserById([FromRoute] int id)
    {
        User? user = await userRepository.GetSingleAsync(id);
        if (user == null)
        {
            return Results.NotFound();
        }

        // Return DTO without password for security
        UserDTO userDTO = new()
        {
            Id = user.Id,
            Username = user.Username
        };
        return Results.Ok(userDTO);
    }
    
    [HttpGet]
    public async Task<IResult> GetUsers([FromQuery] string? UsernameContains = null)
    {
        var users = await userRepository.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(UsernameContains))
        {
            users = users.Where(u => u.Username.Contains(UsernameContains, StringComparison.OrdinalIgnoreCase));
        }

        var userList = await users.ToListAsync();

        // Map to DTOs without passwords for security
        var userDTOs = userList.Select(u => new UserDTO
        {
            Id = u.Id,
            Username = u.Username
        }).ToList();

        return Results.Ok(userDTOs);
    }
    
    // Get posts by a specific user
    [HttpGet("{userId:int}/posts")]
    public async Task<IResult> GetUserPosts([FromRoute] int userId)
    {
        // Check if user exists
        var user = await userRepository.GetSingleAsync(userId);
        if (user == null)
        {
            return Results.NotFound($"User with ID {userId} not found");
        }

        var postsQuery = await postRepository.GetManyAsync();
        var posts = await postsQuery
            .Where(p => p.UserId == userId)
            .ToListAsync();

        // Map to DTOs
        var postDTOs = posts.Select(p => new PostDTO
        {
            Id = p.Id,
            Title = p.Title,
            Body = p.Body,
            UserId = p.UserId
        }).ToList();

        return Results.Ok(postDTOs);
    }

    [HttpGet("{userId:int}/posts/{postId:int}")]
    public async Task<IResult> GetUserPost([FromRoute] int userId, [FromRoute] int postId)
    {
        // Check if user exists
        var user = await userRepository.GetSingleAsync(userId);
        if (user == null)
        {
            return Results.NotFound($"User with ID {userId} not found");
        }

        var postsQuery = await postRepository.GetManyAsync();
        var post = await postsQuery
            .FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);

        if (post == null)
        {
            return Results.NotFound($"Post with ID {postId} not found for user {userId}");
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

    [HttpDelete("{id:int}")]
    public async Task<IResult> DeleteUser([FromRoute] int id)
    {
        await userRepository.DeleteAsync(id);
        return Results.NoContent();
    }
}