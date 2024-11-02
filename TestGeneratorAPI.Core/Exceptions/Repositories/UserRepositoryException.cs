namespace TestGeneratorAPI.Core.Exceptions.Repositories;

public class UserRepositoryException : RepositoryException
{
    public UserRepositoryException(string message) : base(message) { }
    public UserRepositoryException(string message, Exception innerException) : base(message, innerException) { }
}
