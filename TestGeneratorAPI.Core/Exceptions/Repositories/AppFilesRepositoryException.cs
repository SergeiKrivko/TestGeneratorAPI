namespace TestGeneratorAPI.Core.Exceptions.Repositories;

public class AppFilesRepositoryException : RepositoryException
{
    public AppFilesRepositoryException(string message) : base(message) { }
    public AppFilesRepositoryException(string message, Exception innerException) : base(message, innerException) { }
}
