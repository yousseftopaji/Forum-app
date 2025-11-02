using Entities;
using RepositoryContracts;
using System.Text.Json;

namespace FileRepository;

public class UserInFileRepository : IUserRepository
{
    private readonly string filePath = "users.json";

    public UserInFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<User>> GetUsersAsync()
    {
        string usersAsJson = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<User>>(usersAsJson)!;
    }

    private async Task SaveUsersAsync(List<User> users)
    {
        string usersAsJson = JsonSerializer.Serialize(users);
        await File.WriteAllTextAsync(filePath, usersAsJson);
    }

    public async Task<User> AddAsync(User user)
    {
        List<User> users = await GetUsersAsync();
        user.Id = users.Count != 0 ? users.Max(u => u.Id) + 1 : 1;
        users.Add(user);
        await SaveUsersAsync(users);
        return user;
    }

    public async Task DeleteAsync(int id)
    {
        List<User> users = await GetUsersAsync();
        User? userToRemove = users.SingleOrDefault(u => u.Id == id) ?? throw new InvalidOperationException($"User with id '{id}' not found");
        users.Remove(userToRemove);
        await SaveUsersAsync(users);
    }

    public async Task<IQueryable<User>> GetManyAsync()
    {
        List<User> users = await GetUsersAsync();
        return users.AsQueryable();
    }

    public async Task<User> GetSingleAsync(int id)
    {
        List<User> users = await GetUsersAsync();
        User? user = users.SingleOrDefault(u => u.Id == id) ?? throw new InvalidOperationException($"User with id '{id}' not found");
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        List<User> users = await GetUsersAsync();
        User? existingUser = users.SingleOrDefault(u => u.Id == user.Id) ?? throw new InvalidOperationException($"User with id '{user.Id}' not found");
        users.Remove(existingUser);
        users.Add(user);
        await SaveUsersAsync(users);
    }
}
