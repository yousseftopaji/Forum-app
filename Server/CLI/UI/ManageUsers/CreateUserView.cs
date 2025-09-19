using RepositoryContracts;
using Entities;

namespace CLI.UI.ManageUsers;

public class CreateUserView
{
    private readonly IUserRepository _userRepository;

    public CreateUserView(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> ShowAsync()
    {
        Console.Write("Enter user name: ");
        var name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("User name cannot be empty.");
            return null;
        }

        Console.Write("Enter password: ");
        var password = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Password cannot be empty.");
            return null;
        }

        var user = await _userRepository.AddAsync(new User { Username = name, Password = password, Id = 0});
        if (user == null)
        {
            Console.WriteLine("Failed to create user.");
            return null;
        }
        return user;
    }
}