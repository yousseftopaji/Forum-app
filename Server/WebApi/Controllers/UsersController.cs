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
        await VerifyUsernameIsUnique(request.Username);
        User user = new()
        {
            Username = request.Username,
            Password = request.Password
        };

        User created = await userRepository.AddAsync(user);
        UserDTO userDTO = new()
        {
            Username = created.Username,
            Password = created.Password
        };
        return Created($"/users/{created.Id}", userDTO);
    }

    private async Task VerifyUsernameIsUnique(string username)
    {
        var existingUser = userRepository.GetManyAsync()
            .FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

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

        existing.Username = request.Username ?? existing.Username;
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
        return Results.Ok(user);
    }

    [HttpGet]
    public IResult GetUsers([FromQuery] string? UsernameContains = null)
    {
        var users = userRepository.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(UsernameContains))
        {
            users = users.Where(u => u.Username.Contains(UsernameContains, StringComparison.OrdinalIgnoreCase));
        }

        return Results.Ok(users.ToList());
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

        var posts = postRepository.GetManyAsync()
            .Where(p => p.UserId == userId)
            .ToList();
            
        return Results.Ok(posts);
    }

    [HttpDelete("{id:int}")]
    public async Task<IResult> DeleteUser([FromRoute] int id)
    {
        await userRepository.DeleteAsync(id);
        return Results.NoContent();
    }
}