namespace DefaultNamespace;

using RepositoryContracts;
using Entities;

public class CreatePostView
{
    private readonly IPostRepository _postRepository;
    private readonly User _loggedInUser;
    
    public CreatePostView(IPostRepository postRepository, User loggedInUser)
    {
        this._postRepository = postRepository;
        this._loggedInUser = loggedInUser;
    }

    public async Task StartAsync()
    {
        Console.Write("Enter post title: ");
        var title = Console.ReadLine();
        Console.Write("Enter post body: ");
        var body = Console.ReadLine();
  
        var newPost = new Post
        {
            Title = title,
            Body = body,
            UserId = _loggedInUser.Id
        };
        
        await _postRepository.AddAsync(newPost);
        Console.WriteLine("Post created successfully.");
    }
}