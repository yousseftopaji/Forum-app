using RepositoryContracts;
using Entities;

namespace CLI.UI.ManageUsers;

public class LoginUserView
{
    private readonly IUserRepository _userRepository;
    
    public LoginUserView(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<User?> ShowAsync()
    {
        Console.Write("Enter username: ");
        var username = Console.ReadLine();
        Console.Write("Enter password: ");
        var password = Console.ReadLine();

        var user = _userRepository.GetManyAsync()
            .FirstOrDefault(u => u.Username == username && u.Password == password);

        if (user != null)
        {
            Console.WriteLine("Login successful!");
            return user;
        }
        else
        {
            Console.WriteLine("Invalid username or password.");
            return null;
        }
    }
}