namespace Domain.Exceptions
{
    public class BookAlreadyReturnedException(string message) : Exception(message);
}
