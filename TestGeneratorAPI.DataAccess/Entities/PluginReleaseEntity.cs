namespace TestGeneratorAPI.DataAccess.Entities;

public class PluginReleaseEntity
{
    public Guid PluginId { get; init; }
    
    public Guid PluginReleaseId { get; init; }

    public Guid PublisherId { get; init; }

    public string Name { get; init; } = string.Empty;
    
    public string Description { get; init; } = string.Empty;
    
    public required string Version { get; init; }
    
    public string? Url { get; init; }
    
    public string? Runtime { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; init; }
    
    public virtual UserEntity Publisher { get; init; }
    
    public virtual PluginEntity Plugin { get; init; }
}