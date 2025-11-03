using System;
using DTOs.Models;

namespace BlazorApp1.Services;

public interface IUserService
{
    public Task<UserDTO> AddUserAsync(CreateUserDTO request);

    public Task UpdateUserAsync(int id, UpdateUserDTO request);

    public Task<UserDTO> GetUserByIdAsync(int id);

    public Task<List<UserDTO>> GetUsersAsync(string? usernameContains = null);

    public Task<List<PostDTO>> GetUserPostsAsync(int userId);

    public Task<PostDTO> GetUserPostAsync(int userId, int postId);
    
    public Task DeleteUserAsync(int id);
}
