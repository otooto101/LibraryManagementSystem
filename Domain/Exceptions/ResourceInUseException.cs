namespace Domain.Exceptions
{
    public class ResourceInUseException(string message, Exception innerException)
            : Exception(message, innerException)
    {
    }
}
