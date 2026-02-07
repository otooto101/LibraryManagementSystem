namespace Domain.Exceptions
{
    public class DuplicateResourceException(string message, Exception innerException)
            : Exception(message, innerException);
}
