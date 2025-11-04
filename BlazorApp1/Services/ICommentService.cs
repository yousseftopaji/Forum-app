using System;
using DTOs.Models;

namespace BlazorApp1.Services;

public interface ICommentService
{
    public Task<CommentDTO> AddCommentAsync(CreateCommentDTO request, int postId);
    public Task UpdateCommentAsync(int id, UpdateCommentDTO request);
    public Task UpdateCommentOfPostAsync(int postId, int commentId, UpdateCommentDTO request);
    public Task<CommentDTO> GetCommentByIdAsync(int id);
    public Task<List<CommentDTO>> GetAllCommentsAsync();
    public Task DeleteCommentAsync(int id);
    public Task DeleteCommentOfPostAsync(int postId, int commentId);
}
