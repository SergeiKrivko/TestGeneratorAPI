using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Core.Abstractions;

public interface IUsersService
{
    public Task<UserRead> Get(string login);
    
    public Task<AuthorizedUserRead?> GetAuthorized(string login, string password);
    
    Task<Guid> CreateUser(UserCreate user);

    // Task<UserRead> GetUser(Guid userId);

    // Task<UserRead> UpdateUser(Guid userId, UserCreate user);

    // Task DeleteUser(Guid userId);
}