using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepository;

public class CommentInFileRepository : ICommentRepository
{
    private readonly string filePath = "comments.json";

    public CommentInFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<Comment>> GetCommentsAsync()
    {
        string commentsAsJson = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
    }

    private async Task SaveCommentsAsync(List<Comment> comments)
    {
        string commentsAsJson = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, commentsAsJson);
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        List<Comment> comments = await GetCommentsAsync();
        int maxId = comments.Count > 0 ? comments.Max(c => c.Id) : 1;
        comment.Id = maxId + 1;
        comments.Add(comment);
        await SaveCommentsAsync(comments);
        return comment;
    }

    public async Task DeleteAsync(int id)
    {
        List<Comment> comments = await GetCommentsAsync();
        Comment? commentToRemove = comments.SingleOrDefault(c => c.Id == id) ?? throw new InvalidOperationException($"Comment with id '{id}' not found");
        comments.Remove(commentToRemove);
        await SaveCommentsAsync(comments);
    }

    public async Task<IQueryable<Comment>> GetManyAsync()
    {
        List<Comment> comments = await GetCommentsAsync();
        return comments.AsQueryable();
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        List<Comment> comments = await GetCommentsAsync();
        Comment? comment = comments.SingleOrDefault(c => c.Id == id) ?? throw new InvalidOperationException($"Comment with id '{id}' not found");
        return comment;
    }

    public async Task UpdateAsync(Comment comment)
    {
        List<Comment> comments = await GetCommentsAsync();
        Comment? existingComment = comments.SingleOrDefault(c => c.Id == comment.Id) ?? throw new InvalidOperationException($"Comment with id '{comment.Id}' not found");
        comments.Remove(existingComment);
        comments.Add(comment);
        await SaveCommentsAsync(comments);
    }
}
