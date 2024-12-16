using TestGeneratorAPI.Core.Enums;

namespace TestGeneratorAPI.Core.Models;

public class TokenRead
{
    public Guid TokenId { get; init; }

    public Guid UserId { get; init; }

    public string Name { get; init; } = string.Empty;

    public required DateTime CreatedAt { get; init; }
    
    public DateTime ExpiresAt { get; init; }

    public DateTime? DeletedAt { get; init; }
    
    public required TokenType Type { get; init; }
    
    public required string[] Permissions { get; init; }
}