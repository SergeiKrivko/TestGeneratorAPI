namespace TestGeneratorAPI.Core.Exceptions.Services;

public class PluginsServiceException : ServiceException
{
    public PluginsServiceException(string message) : base(message) { }
    public PluginsServiceException(string message, Exception innerException) : base(message, innerException) { }
}