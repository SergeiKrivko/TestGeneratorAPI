namespace TestGeneratorAPI.Core.Models;

public class AppFileDownload
{
    public required string Filename { get; init; }
    public required string Hash { get; init; }
}