using System.ComponentModel.DataAnnotations;

namespace TestGeneratorAPI.DataAccess.Entities;

public class AppFileEntity
{
    public Guid Id { get; init; }

    public required Guid ReleaseId { get; init; }
    
    public required Guid S3Id { get; init; }

    [MaxLength(60)] public required string Filename { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; init; }

    [MaxLength(200)] public required string Hash { get; init; }
    
    public virtual ReleaseEntity Release { get; init; }
}