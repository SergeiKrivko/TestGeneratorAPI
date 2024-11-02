namespace TestGeneratorAPI.DataAccess.Entities;

public class PluginEntity
{
    public Guid PluginId { get; init; }

    public Guid OwnerId { get; init; }

    public required string Key { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; init; }

    public virtual ICollection<PluginReleaseEntity> Releases { get; set; } = [];
    
    public virtual UserEntity Owner { get; set; }
}