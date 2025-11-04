using System;
using System.Net.Http.Json;
using System.Text.Json;
using DTOs.Models;

namespace BlazorApp1.Services;

public class HttpCommentService : ICommentService
{
    private readonly HttpClient client;

    public HttpCommentService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<CommentDTO> AddCommentAsync(CreateCommentDTO request, int postId)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync($"/posts/{postId}/comments", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<CommentDTO>(response, JsonOptions())!;
    }

    public async Task DeleteCommentAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.DeleteAsync($"/comments/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }

    public async Task DeleteCommentOfPostAsync(int postId, int commentId)
    {
        HttpResponseMessage httpResponse = await client.DeleteAsync($"/posts/{postId}/comments/{commentId}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }

    public async Task<List<CommentDTO>> GetAllCommentsAsync()
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"/comments");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<List<CommentDTO>>(response, JsonOptions())!;
    }

    public async Task<CommentDTO> GetCommentByIdAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"/comments/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<CommentDTO>(response, JsonOptions())!;
    }

    public async Task UpdateCommentAsync(int id, UpdateCommentDTO request)
    {
        HttpRequestMessage httpRequest = new(HttpMethod.Patch, $"/comments/{id}")
        {
            Content = JsonContent.Create(request)
        };

        HttpResponseMessage httpResponse = await client.SendAsync(httpRequest);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
    }

    public async Task UpdateCommentOfPostAsync(int postId, int commentId, UpdateCommentDTO request)
    {
        HttpResponseMessage httpResponse = await client.PatchAsJsonAsync($"/posts/{postId}/comments/{commentId}", request);
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
