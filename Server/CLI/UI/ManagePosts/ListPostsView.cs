using RepositoryContracts;
using Entities;

namespace DefaultNamespace;

public class ListPostsView
{
    private readonly IPostRepository _postRepository;

    public ListPostsView(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<int?> ShowAsync()
    {
        var posts = _postRepository.GetManyAsync();
        Console.WriteLine("List of posts:");
        foreach (var post in posts)
        {
            Console.WriteLine($"- {post.Title} (ID: {post.Id})");
        }
        await Task.CompletedTask;

        Console.Write("Enter post ID to view details or '0' to go back: ");
        var input = Console.ReadLine();
        if (int.TryParse(input, out int postId) && postId != 0)
        {
            var selectedPost = posts.FirstOrDefault(p => p.Id == postId);
            if (selectedPost != null)
            {
                return selectedPost.Id;
            }
            else
            {
                Console.WriteLine("Post not found.");
            }
        }
        return null;
    }
}