namespace MailingManager.Core.Exceptions.Services;

public class DataNotFoundException : Exception
{
    public DataNotFoundException(string message) : base(message) { }
    
    public DataNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}