namespace TestGeneratorAPI.Core.Models;

public class PluginReleaseRead
{
    public Guid PluginReleaseId { get; init; }
    public Guid PluginId { get; init; }
    public Guid PublisherId { get; init; }

    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public required Version Version { get; init; }
    public string? Runtime { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
    
    public string? Url { get; init; }
}