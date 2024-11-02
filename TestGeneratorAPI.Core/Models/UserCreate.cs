namespace TestGeneratorAPI.Core.Models;

public class UserCreate
{
    public required string Login { get; init; }
    
    public string? Name { get; init; } = null;
    
    public required string Password { get; init; }
}