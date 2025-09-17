using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{
    private List<Comment> comments;
    
    public CommentInMemoryRepository()
    {
        comments = GetDummyComments();
    }
    
    private List<Comment>? GetDummyComments()
    {
        return
        [
            new Comment { Body = "Comment 1", PostId = 1, Id = 1, UserId = 2},
            new Comment { Body = "Comment 2", PostId = 2, Id = 2, UserId = 1},
            new Comment { Body = "Comment 3", PostId = 3, Id = 3, UserId = 3}
        ];
    }

    public Task<Comment> AddAsync(Comment comment)
    {
        comment.Id = comments.Any() ? comments.Max(c => c.Id) + 1 : 1;
        comments.Add(comment);
        return Task.FromResult(comment);
    }

    public Task UpdateAsync(Comment comment)
    {
        Comment? oldComment = comments.SingleOrDefault(c => c.Id == comment.Id);
        if (oldComment is null)
        {
            throw new InvalidOperationException($"Comment with id {comment.Id} not found");
        }

        comments.Remove(oldComment);
        comments.Add(comment);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Comment? comment = comments.SingleOrDefault(c => c.Id == id);
        if (comment is null)
        {
            throw new InvalidOperationException($"Comment with id {id} not found");
        }
        comments.Remove(comment);
        return Task.CompletedTask;
    }

    public Task<Comment> GetSingleAsync(int id)
    {
        Comment? comment = comments.SingleOrDefault(c => c.Id == id);
        if (comment is null)
        {
            throw new InvalidOperationException($"Comment with id {id} not found");
        }
        
        return Task.FromResult(comment);
    }

    public IQueryable<Comment> GetManyAsync()
    {
        return comments.AsQueryable();
    }
}