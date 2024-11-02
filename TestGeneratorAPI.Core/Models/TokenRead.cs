namespace TestGeneratorAPI.Core.Models;

public class TokenRead
{
    public Guid TokenId { get; init; }
    
    public Guid UserId { get; init; }

    public string Name { get; init; } = string.Empty;

    public required DateTime CreatedAt { get; init; }

    public DateTime? DeletedAt { get; init; }
}