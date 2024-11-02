namespace TestGeneratorAPI.Core.Models;

public class PluginRead
{
    public Guid PluginId { get; set; }
    public Guid OwnerId { get; set; }

    public string Key { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}