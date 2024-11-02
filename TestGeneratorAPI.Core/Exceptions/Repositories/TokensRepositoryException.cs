namespace TestGeneratorAPI.Core.Exceptions.Repositories;

public class TokensRepositoryException : RepositoryException
{
    public TokensRepositoryException(string message) : base(message) { }
    public TokensRepositoryException(string message, Exception innerException) : base(message, innerException) { }
}
