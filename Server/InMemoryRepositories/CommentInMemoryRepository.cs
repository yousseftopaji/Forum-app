using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{
    private readonly List<Comment> _comments = [];
    
    public CommentInMemoryRepository()
    {
        SeedDataAsync().GetAwaiter();
    }
    
    public Task<Comment> AddAsync(Comment comment)
    {
        comment.Id = _comments.Count != 0 ? _comments.Max(c => c.Id) + 1 : 1;
        _comments.Add(comment);
        return Task.FromResult(comment);
    }

    public Task UpdateAsync(Comment comment)
    {
        Comment? oldComment = _comments.SingleOrDefault(c => c.Id == comment.Id);
        if (oldComment is null)
        {
            throw new InvalidOperationException($"Comment with id {comment.Id} not found");
        }

        _comments.Remove(oldComment);
        _comments.Add(comment);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var comment = _comments.SingleOrDefault(c => c.Id == id) ?? throw new InvalidOperationException($"Comment with ID '{id}' not found.");
        _comments.Remove(comment);
        return Task.CompletedTask;
    }

    public Task<Comment> GetSingleAsync(int id)
    {
        var comment = _comments.SingleOrDefault(c => c.Id == id) ?? throw new InvalidOperationException($"Comment with id {id} not found");
        return Task.FromResult(comment);
    }

    public Task<IQueryable<Comment>> GetManyAsync()
    {
        return Task.FromResult(_comments.AsQueryable());
    }
    
    private async Task SeedDataAsync()
    {
        Comment comment1 = new()
        {
            Id = 1,
            Body = "comment1",
            PostId = 1,
            UserId = 1,
        };
        Comment comment2 = new()
        {
            Id = 2,
            Body = "comment2",
            PostId = 2,
            UserId = 2,
        };
        Comment comment3 = new()
        {
            Id = 3,
            Body = "comment3",
            PostId = 2,
            UserId = 2,
        };
        await AddAsync(comment1);
        await AddAsync(comment2);
        await AddAsync(comment3);
    }
}