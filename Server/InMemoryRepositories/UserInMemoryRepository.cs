﻿using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class UserInMemoryRepository : IUserRepository
{
    private List<User> users;

    public UserInMemoryRepository()
    {
        users = GetDummyUsers();
    }

    private List<User>? GetDummyUsers()
    {
        return
        [
            new User { Username = "Youssef", Password = "123", Id = 1 },
            new User { Username = "Ahmed", Password = "456", Id = 2 },
            new User { Username = "Ali", Password = "789", Id = 3 }
        ];
    }

    public Task<User> AddAsync(User user)
    {
        user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
        users.Add(user);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        User? existingUser = users.SingleOrDefault(u => u.Id == user.Id);
        if (existingUser is null)
        {
            throw new InvalidOperationException($"User with id '{user.Id}' not found");
        }

        users.Remove(existingUser);
        users.Add(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        User? userToRemove = users.SingleOrDefault(u => u.Id == id);
        if (userToRemove is null)
        {
            throw new InvalidOperationException($"User with id '{id}' not found"); 
        }
        
        users.Remove(userToRemove);
        return Task.CompletedTask;
    }

    public Task<User> GetSingleAsync(int id)
    {
        User? user = users.SingleOrDefault(u => u.Id == id);
        return user is null ? throw new InvalidOperationException($"User with id '{id}' not found") : Task.FromResult(user);
    }

    public IQueryable<User> GetManyAsync()
    {
        return users.AsQueryable();
    }
}