using Entities;

namespace RepositoryContracts;

public interface IUserRepository
{
    Task<User>  AddAsync(User user);
    Task<User>  UpdateAsync(User user);
    Task<User>  DeleteAsync(int id);
    Task<User>  GetSingleAsync(int id);
    IQueryable<User> GetManyAsync();
}