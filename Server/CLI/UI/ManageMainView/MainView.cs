namespace CLI.UI.ManageMainView;

using RepositoryContracts;
using Entities;
using DefaultNamespace;

public class MainView
{
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly User _user;

    public MainView(IUserRepository userRepository, IPostRepository postRepository, ICommentRepository commentRepository, User user)
    {
        _userRepository = userRepository;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _user = user;
    }

    public async Task ShowAsync()
    {
        Console.WriteLine($"You are logged in as {_user.Username}, ID: {_user.Id}!");

        // main view functionality
        Console.WriteLine("------- Main View -------");
        Console.WriteLine("1. View Posts");
        Console.WriteLine("2. Create Post");
        Console.WriteLine("0. Log out");
        Console.Write("Select an option: ");
        var input = Console.ReadLine();

        switch (input)
        {
            case "1":
                var managePostsView = new ManagePostsView(_postRepository, _commentRepository, _user);
                await managePostsView.ShowAsync();
                MainView mainViewAfterList = new(_userRepository, _postRepository, _commentRepository, _user);
                await mainViewAfterList.ShowAsync();
                break;
            case "2":
                CreatePostView createPostView = new(_postRepository, _user);
                await createPostView.StartAsync();
                MainView mainViewAfterPost = new(_userRepository, _postRepository, _commentRepository, _user);
                await mainViewAfterPost.ShowAsync();
                break;
            case "0":
                Console.WriteLine("Logging out...");
                return;
            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
        await Task.CompletedTask;
    }
}