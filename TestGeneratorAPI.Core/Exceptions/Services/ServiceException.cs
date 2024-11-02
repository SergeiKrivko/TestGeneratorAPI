namespace TestGeneratorAPI.Core.Exceptions.Services;

public class ServiceException : Exception
{
    protected ServiceException(string message) : base(message) { }
    protected ServiceException(string message, Exception innerException) : base(message, innerException) { }
}