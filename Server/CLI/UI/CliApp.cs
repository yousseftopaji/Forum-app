namespace CLI.UI;

using CLI.UI.ManageUsers;
using RepositoryContracts;
using Entities;
using CLI.UI.ManageMainView;

public class CliApp
{
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;

    
    public CliApp(IUserRepository userRepository, IPostRepository postRepository, ICommentRepository commentRepository)
    {
        _userRepository = userRepository;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
    }

    
    public async Task StartAsync()
    {
        Console.WriteLine("CLI started, ready to accept commands.");

        while (true)
        {
            Console.WriteLine("\n--- Main Menu ---");
            Console.WriteLine("1. Log in");
            Console.WriteLine("2. Register");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    var loginView = new LoginUserView(_userRepository);
                    User? user = await loginView.ShowAsync();
                    if (user != null)
                    {
                        var mainView = new MainView(_userRepository, _postRepository, _commentRepository, user); 
                        await mainView.ShowAsync();
                    }
                    break;
                case "2":
                    var createUserView = new CreateUserView(_userRepository);
                    await createUserView.ShowAsync();
                    
                    break;
                case "0":
                    Console.WriteLine("Exiting application. Goodbye!");
                    break;
            }
            if (input == "0") break;
        }
    }
}