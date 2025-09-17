using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class PostInMemoryRepository : IPostRepository
{
    private List<Post> posts;
    
    public PostInMemoryRepository()
    {
        posts = GetDummyPosts();
    }
    
    private List<Post>? GetDummyPosts()
    {
        return
        [
            new Post { Title = "Post 1", Body = "Content 1", Id = 1, UserId = 2 },
            new Post { Title = "Post 2", Body = "Content 2", Id = 2, UserId = 1 },
            new Post { Title = "Post 3", Body = "Content 3", Id = 3, UserId = 3 }
        ];
    }
    
    public Task<Post> AddAsync(Post post)
    {
        post.Id = posts.Any() ? posts.Max(p => p.Id) + 1 : 1;
        posts.Add(post);
        return Task.FromResult(post);
    }

    public Task UpdateAsync(Post post)
    {
        Post? existingPost = posts.SingleOrDefault(p => p.Id == post.Id);
        if (existingPost is null)
        {
            throw new InvalidOperationException($"Post with id '{post.Id}' not found");
        }

        posts.Remove(existingPost);
        posts.Add(post);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Post? postToRemove = posts.SingleOrDefault(p => p.Id == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException($"Post with id '{id}' not found"); 
        }
        
        posts.Remove(postToRemove);
        return Task.CompletedTask;
    }

    public Task<Post> GetSingleAsync(int id)
    {
        Post? post = posts.SingleOrDefault(p => p.Id == id);
        return post is null ? throw new InvalidOperationException($"Post with id '{id}' not found") : Task.FromResult(post);
    }

    public IQueryable<Post> GetManyAsync()
    {
        return posts.AsQueryable();
    }
}