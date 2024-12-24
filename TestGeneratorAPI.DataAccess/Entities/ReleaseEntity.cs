using System.ComponentModel.DataAnnotations;

namespace TestGeneratorAPI.DataAccess.Entities;

public class ReleaseEntity
{
    public Guid ReleaseId { get; init; }

    [MaxLength(20)] public required string Version { get; init; }

    [MaxLength(10)] public required string Runtime { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public virtual ICollection<AppFileEntity> Files { get; set; } = [];
}