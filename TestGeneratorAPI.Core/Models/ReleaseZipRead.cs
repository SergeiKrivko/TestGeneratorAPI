namespace TestGeneratorAPI.Core.Models;

public class ReleaseZipRead
{
    public required string Url { get; init; }
    public string[] DeletedFiles { get; init; } = [];
}