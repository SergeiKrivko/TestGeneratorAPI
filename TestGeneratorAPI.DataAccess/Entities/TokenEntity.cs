namespace TestGeneratorAPI.DataAccess.Entities;

public class TokenEntity
{
    public Guid TokenId { get; init; }

    public required Guid UserId { get; init; }

    public string Name { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; init; } = null;
}