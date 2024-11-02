using TestGeneratorAPI.Core.Models;

namespace TestGeneratorAPI.Core.Abstractions;

public interface ITokensService
{
    Task<string> CreateToken(TokenCreate tokenCreate, Guid userId);

    Task<bool> IsAlive(Guid id);

    // Task<UserRead> GetUser(Guid userId);

    // Task<UserRead> UpdateUser(Guid userId, UserCreate user);

    // Task DeleteUser(Guid userId);
}