using RepositoryContracts;
using Entities;
namespace DefaultNamespace;

public class ListUsersView
{
    private readonly IUserRepository _userRepository;

    public ListUsersView(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        var users = _userRepository.GetManyAsync();
        Console.WriteLine("List of users:");
        foreach (var user in users)
        {
            Console.WriteLine($"- {user.Username} (ID: {user.Id})");
        }
        await Task.CompletedTask;
    }
}