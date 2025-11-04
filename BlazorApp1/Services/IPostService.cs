using System;
using DTOs.Models;

namespace BlazorApp1.Services;

public interface IPostService
{
    public Task<PostDTO> AddPostAsync(CreatePostDTO request);
    public Task UpdatePostAsync(int id, UpdatePostDTO request);
    public Task<PostDTO> GetPostByIdAsync(int id);
    public Task<List<PostDTO>> GetPostsAsync(string? titleContains = null);
    public Task<List<CommentDTO>> GetPostCommentsAsync(int postId);
    public Task DeletePostAsync(int id);
}
