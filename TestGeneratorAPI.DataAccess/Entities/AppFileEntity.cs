namespace TestGeneratorAPI.DataAccess.Entities;

public class AppFileEntity
{
    public Guid Id { get; init; }

    public required string Filename { get; init; } = string.Empty;
    
    public required string Version { get; init; } = string.Empty;
    
    public required string Runtime { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; init; }
    
    public required string Hash { get; init; }
}