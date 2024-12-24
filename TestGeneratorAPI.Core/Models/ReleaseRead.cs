namespace TestGeneratorAPI.Core.Models;

public class ReleaseRead
{
    public required Guid ReleaseId { get; init; }
    public required Version Version { get; init; }
    public required string Runtime { get; init; }
    public required DateTime CreatedAt { get; init; }
}