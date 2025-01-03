using TestGeneratorAPI.Core.Enums;
using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Core.Abstractions;

public interface ITokensRepository
{
    Task<List<TokenRead>> GetAll(Guid userId);

    Task<TokenRead> Get(Guid tokenId);
    
    // Task<UserRead?> GetLastByLogin(string login);

    Task<Guid> Create(Guid id, Guid userId, TokenType type, string name, DateTime expiresAt, string[] permissions);

    // Task<Guid> Update(Guid id, string passwordHash, string? name);

    Task<Guid> Delete(Guid tokenId);
}