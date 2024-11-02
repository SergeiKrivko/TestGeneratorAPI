namespace TestGeneratorAPI.Core.Exceptions.Repositories;

public class RepositoryException : Exception
{
    protected RepositoryException(string message) : base(message) { }
    protected RepositoryException(string message, Exception innerException) : base(message, innerException) { }
}