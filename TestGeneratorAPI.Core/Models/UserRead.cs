namespace TestGeneratorAPI.Core.Models;

public class UserRead
{
    public Guid UserId { get; init; }
    
    public string Name { get; init; } = string.Empty;
    
    public string Login { get; init; } = string.Empty;
    
    public required DateTime CreatedAt { get; init; }
    
    public required DateTime UpdatedAt { get; init; }
}