using System;
using System.Text.Json;
using DTOs.Models;

namespace BlazorApp1.Services;

public class HttpUserService : IUserService
{
    private readonly HttpClient client;

    public HttpUserService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<UserDTO> AddUserAsync(CreateUserDTO request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("users", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<UserDTO>(response, JsonOptions())!;
    }

    public async Task DeleteUserAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.DeleteAsync($"users/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return;
    }

    public async Task<UserDTO> GetUserByIdAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"users/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<UserDTO>(response, JsonOptions())!;
    }

    public async Task<PostDTO> GetUserPostAsync(int userId, int postId)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"users/{userId}/posts/{postId}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<PostDTO>(response, JsonOptions())!;
    }

    public async Task<List<PostDTO>> GetUserPostsAsync(int userId)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"users/{userId}/posts");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<List<PostDTO>>(response, JsonOptions())!;
    }

    public async Task<List<UserDTO>> GetUsersAsync(string? usernameContains = null)
    {
        string url = string.IsNullOrEmpty(usernameContains) 
            ? "users" 
            : $"users?usernameContains={usernameContains}";
        
        HttpResponseMessage httpResponse = await client.GetAsync(url);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<List<UserDTO>>(response, JsonOptions())!;
    }

    public async Task UpdateUserAsync(int id, UpdateUserDTO request)
    {
        HttpResponseMessage httpResponse = await client.PatchAsJsonAsync($"users/{id}", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return;
    }

    private JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }
}
