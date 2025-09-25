using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class UserInMemoryRepository : IUserRepository
{
    private readonly List<User> users = [];

    public UserInMemoryRepository()
    {
        SeedDataAsync().GetAwaiter();
    }

    private async Task SeedDataAsync()
    {
        User user1 = new() { Username = "Youssef", Password = "123", Id = 1 };
        User user2 = new() { Username = "Ahmed", Password = "456", Id = 2 };
        User user3 = new() { Username = "Ali", Password = "789", Id = 3 };

        await AddAsync(user1);
        await AddAsync(user2);
        await AddAsync(user3);
    }

    public Task<User> AddAsync(User user)
    {
        user.Id = users.Count != 0 ? users.Max(u => u.Id) + 1 : 1;
        users.Add(user);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        User? existingUser = users.SingleOrDefault(u => u.Id == user.Id) ?? throw new InvalidOperationException($"User with id '{user.Id}' not found");
        users.Remove(existingUser);
        users.Add(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        User? userToRemove = users.SingleOrDefault(u => u.Id == id) ?? throw new InvalidOperationException($"User with id '{id}' not found");
        users.Remove(userToRemove);
        return Task.CompletedTask;
    }

    public Task<User> GetSingleAsync(int id)
    {
        User? user = users.SingleOrDefault(u => u.Id == id) ?? throw new InvalidOperationException($"User with id '{id}' not found");
        return Task.FromResult(user);
    }

    public IQueryable<User> GetManyAsync()
    {
        return users.AsQueryable();
    }
}