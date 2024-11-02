namespace TestGeneratorAPI.Core.Models;

public class PluginReleaseRead
{
    public Guid PluginReleaseId { get; set; }
    public Guid PluginId { get; set; }
    public Guid PublisherId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public required Version Version { get; set; }
    public string? Runtime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}