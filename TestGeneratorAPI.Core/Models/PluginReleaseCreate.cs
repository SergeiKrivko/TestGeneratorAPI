namespace TestGeneratorAPI.Core.Models;

public class PluginReleaseCreate
{
    public required string Key { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public required Version Version { get; set; }
    public string? Runtime { get; set; }
    
    public string? Url { get; set; }
}