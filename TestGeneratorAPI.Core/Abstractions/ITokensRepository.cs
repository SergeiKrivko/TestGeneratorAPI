using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Core.Abstractions;

public interface ITokensRepository
{
    // Task<List<UserRead>> GetAll();

    Task<TokenRead> Get(Guid tokenId);
    
    // Task<UserRead?> GetLastByLogin(string login);

    Task<Guid> Create(Guid id, Guid userId, string name);

    // Task<Guid> Update(Guid id, string passwordHash, string? name);

    // Task<Guid> Delete(Guid userId);
}