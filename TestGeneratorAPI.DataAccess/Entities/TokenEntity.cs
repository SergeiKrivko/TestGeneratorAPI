using TestGeneratorAPI.Core.Enums;

namespace TestGeneratorAPI.DataAccess.Entities;

public class TokenEntity
{
    public Guid TokenId { get; init; }

    public required Guid UserId { get; init; }

    public string Name { get; init; } = string.Empty;
    
    public TokenType Type { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    public DateTime ExpiresAt { get; init; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; set; } = null;

    public string[] Permissions { get; init; } = [];
}