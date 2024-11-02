namespace TestGeneratorAPI.Core.Models;

public class AuthorizedUserRead
{
    public Guid UserId { get; init; }
    
    public string Name { get; init; } = string.Empty;
    
    public string Login { get; init; } = string.Empty;
    
    public string PasswordHash { get; init; } = string.Empty;
    
    public required DateTime CreatedAt { get; init; }
    
    public required DateTime UpdatedAt { get; init; }
    
    public DateTime? DeletedAt { get; init; }
}