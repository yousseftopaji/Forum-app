using System;
using System.Net.Http.Json;
using System.Text.Json;
using DTOs.Models;

namespace BlazorApp1.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;

    public HttpPostService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<PostDTO> AddPostAsync(CreatePostDTO request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync($"/users/{request.UserId}/posts", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<PostDTO>(response, JsonOptions())!;
    }

    public async Task DeletePostAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.DeleteAsync($"/posts/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return;
    }

    public async Task<PostDTO> GetPostByIdAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"/posts/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<PostDTO>(response, JsonOptions())!;
    }

    public async Task<List<CommentDTO>> GetPostCommentsAsync(int postId)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"/posts/{postId}/comments");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<List<CommentDTO>>(response, JsonOptions())!;
    }

    public async Task<List<PostDTO>> GetPostsAsync(string? titleContains = null)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"/posts?titleContains={titleContains}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<List<PostDTO>>(response, JsonOptions())!;
    }

    public async Task UpdatePostAsync(int id, UpdatePostDTO request)
    {
        HttpResponseMessage httpResponse = await client.PatchAsJsonAsync($"/posts/{id}", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }

        private JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }
}
