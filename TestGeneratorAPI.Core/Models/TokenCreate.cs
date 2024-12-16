using TestGeneratorAPI.Core.Enums;

namespace TestGeneratorAPI.Core.Models;

public class TokenCreate
{
    public string Name { get; init; } = string.Empty;
    
    public required TokenType Type { get; init; }
    
    public required string[] Permissions { get; init; }

    public string[]? Plugins { get; init; } = null;
    
    public string? Mask { get; init; } = null;
    
    public DateTime? ExpiresAt { get; init; } = null;
}