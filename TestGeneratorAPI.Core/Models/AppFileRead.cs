namespace TestGeneratorAPI.Core.Models;

public class AppFileRead
{
    public required Guid Id { get; init; }
    public required Guid S3Id { get; init; }
    public required string Filename { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
    public required string Hash { get; init; }
}