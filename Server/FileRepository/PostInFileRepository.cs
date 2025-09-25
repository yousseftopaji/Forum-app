using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepository;

public class PostInFileRepository : IPostRepository
{
    private readonly string filePath = "posts.json";

    public PostInFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<Post>> GetPostsAsync()
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
    }

    private async Task SavePostsAsync(List<Post> posts)
    {
        string postsAsJson = JsonSerializer.Serialize(posts);
        await File.WriteAllTextAsync(filePath, postsAsJson);
    }

    public async Task<Post> AddAsync(Post post)
    {
        List<Post> posts = await GetPostsAsync();
        post.Id = posts.Count != 0 ? posts.Max(p => p.Id) + 1 : 1;
        posts.Add(post);
        await SavePostsAsync(posts);
        return post;
    }

    public async Task DeleteAsync(int id)
    {
        List<Post> posts = await GetPostsAsync();
        Post? postToRemove = posts.SingleOrDefault(p => p.Id == id) ?? throw new InvalidOperationException($"Post with id '{id}' not found");
        posts.Remove(postToRemove);
        await SavePostsAsync(posts);
    }

    public IQueryable<Post> GetManyAsync()
    {
        return GetPostsAsync().Result.AsQueryable();
    }       

    public async Task<Post> GetSingleAsync(int id)
    {
        List<Post> posts = await GetPostsAsync();
        Post? post = posts.SingleOrDefault(p => p.Id == id) ?? throw new InvalidOperationException($"Post with id '{id}' not found");
        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        List<Post> posts = await GetPostsAsync();
        Post? existingPost = posts.SingleOrDefault(p => p.Id == post.Id) ?? throw new InvalidOperationException($"Post with id '{post.Id}' not found");
        posts.Remove(existingPost);
        posts.Add(post);
        await SavePostsAsync(posts);
    }
}
