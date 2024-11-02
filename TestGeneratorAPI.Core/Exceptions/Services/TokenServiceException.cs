namespace TestGeneratorAPI.Core.Exceptions.Services;

public class TokenServiceException : ServiceException
{
    public TokenServiceException(string message) : base(message) { }
    public TokenServiceException(string message, Exception innerException) : base(message, innerException) { }
}