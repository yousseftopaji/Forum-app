using RepositoryContracts;
using Entities;

namespace DefaultNamespace;

public class SinglePostView
{
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly User _user;

    public SinglePostView(IPostRepository postRepository, ICommentRepository commentRepository, User user)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _user = user;
    }

    public async Task ShowAsync(int postId)
    {
        var posts = await _postRepository.GetManyAsync();
        var post = posts.FirstOrDefault(p => p.Id == postId);
        if (post == null)
        {
            Console.WriteLine("Post not found.");
            return;
        }

        Console.WriteLine($"Title: {post.Title}");
        Console.WriteLine($"Body: {post.Body}");
        var comments = _commentRepository.GetManyAsync();
        var filteredComments = comments.Where(c => c.PostId == post.Id);
        Console.WriteLine("Comments:");
        foreach (var comment in comments)
        {
            Console.WriteLine($"- {comment.Body} (ID: {comment.Id})");
        }

        Console.Write("Enter comment body to add a comment or '0' to go back: ");
        var commentInput = Console.ReadLine();
        if (commentInput != "0" && !string.IsNullOrWhiteSpace(commentInput))
        {
            var newComment = new Comment
            {
                Body = commentInput,
                PostId = post.Id,
                UserId = _user.Id
            };
            await _commentRepository.AddAsync(newComment);
            Console.WriteLine("Comment added successfully.");
        }
    }
}