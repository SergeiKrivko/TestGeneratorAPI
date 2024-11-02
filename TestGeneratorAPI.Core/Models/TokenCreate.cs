namespace TestGeneratorAPI.Core.Models;

public class TokenCreate
{
    public string Name { get; init; } = string.Empty;
    
    public required string[] Plugins { get; init; }
}