namespace CLI.UI.ManageMainView;

using RepositoryContracts;
using Entities;

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
        Console.WriteLine($"Welcome, {_user.Username}!");
        await Task.CompletedTask;
    }
}