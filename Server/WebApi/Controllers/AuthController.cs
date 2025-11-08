using DTOs.Models;
using DTOs.Models.Login;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using Entities;

namespace WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public AuthController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login([FromBody] LoginRequest loginRequest)
    {
        List<User> users = (List<User>)await _userRepository.GetManyAsync();
        User? user = users.SingleOrDefault(u => u.Username == loginRequest.Username && u.Password == loginRequest.Password);
        if (user is null)
        {
            return Unauthorized("Invalid username or password.");
        }

        return Ok(new UserDTO { Id = user.Id, Username = user.Username });
    }
}

