namespace TestGeneratorAPI.Core.Exceptions.Repositories;

public class PluginsRepositoryException : RepositoryException
{
    public PluginsRepositoryException(string message) : base(message) { }
    public PluginsRepositoryException(string message, Exception innerException) : base(message, innerException) { }
}
