using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Core.Abstractions;

public interface ITokensService
{
    public Task<string> CreateToken(TokenCreate tokenCreate, Guid userId);

    public Task<bool> IsAlive(Guid id);

    public Task<List<TokenRead>> GetTokensOfUser(Guid userId);

    public Task<TokenRead> GetToken(Guid tokenId);
    
    public Task<Guid> DeleteToken(Guid tokenId);

    // Task<UserRead> UpdateUser(Guid userId, UserCreate user);

    
}