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
        Post? postToRemove = posts.SingleOrDefault(p => p.Id == id);
        if (postToRemove is null)
        {
            return;
        }

        posts.Remove(postToRemove);
        await SavePostsAsync(posts);
    }

    public async Task<IEnumerable<Post>> GetManyAsync()
    {
        List<Post> posts = await GetPostsAsync();
        return posts;
    }

    public async Task<Post?> GetSingleAsync(int id)
    {
        List<Post> posts = await GetPostsAsync();
        Post? post = posts.SingleOrDefault(p => p.Id == id);
        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        List<Post> posts = await GetPostsAsync();
        Post? existingPost = posts.SingleOrDefault(p => p.Id == post.Id);
        if (existingPost is null)
        {
            return;
        }

        posts.Remove(existingPost);
        posts.Add(post);
        await SavePostsAsync(posts);
    }
}
