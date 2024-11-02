namespace TestGeneratorAPI.DataAccess.Entities;

public class UserEntity
{
    public Guid UserId { get; init; }

    public required string Login { get; init; } = string.Empty;
    
    public required string PasswordHash { get; init; } = string.Empty;

    public required string Name { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; init; }

    public virtual ICollection<PluginEntity> Plugins { get; init; } = [];

    public virtual ICollection<PluginReleaseEntity> PluginReleases { get; init; } = [];
}