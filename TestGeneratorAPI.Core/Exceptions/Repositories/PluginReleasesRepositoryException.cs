namespace TestGeneratorAPI.Core.Exceptions.Repositories;

public class PluginReleasesRepositoryException : RepositoryException
{
    public PluginReleasesRepositoryException(string message) : base(message) { }
    public PluginReleasesRepositoryException(string message, Exception innerException) : base(message, innerException) { }
}
