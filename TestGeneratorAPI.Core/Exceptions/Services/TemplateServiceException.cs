namespace TestGeneratorAPI.Core.Exceptions.Services;

public class UserServiceException : ServiceException
{
    public UserServiceException(string message) : base(message) { }
    public UserServiceException(string message, Exception innerException) : base(message, innerException) { }
}