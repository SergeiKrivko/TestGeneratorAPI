namespace TestGeneratorAPI.Core.Exceptions.Services;

public class PluginReleasesServiceException : ServiceException
{
    public PluginReleasesServiceException(string message) : base(message) { }
    public PluginReleasesServiceException(string message, Exception innerException) : base(message, innerException) { }
}