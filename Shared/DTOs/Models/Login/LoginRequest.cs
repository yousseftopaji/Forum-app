using System;

namespace DTOs.Models.Login;

public class LoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
