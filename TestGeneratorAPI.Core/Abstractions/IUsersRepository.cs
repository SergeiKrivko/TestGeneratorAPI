using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Core.Abstractions;

public interface IUsersRepository
{
    // Task<List<UserRead>> GetAll();

    Task<AuthorizedUserRead> Get(Guid userId);
    
    Task<AuthorizedUserRead> GetLastByLogin(string login);

    Task<Guid> Create(Guid id, string login, string passwordHash, string name);

    // Task<Guid> Update(Guid id, string passwordHash, string? name);

    // Task<Guid> Delete(Guid userId);
}