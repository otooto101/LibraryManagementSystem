namespace Domain.Exceptions
{
    public class BookAlreadyExistsException(string ISBN)
        : Exception($"Book with ISBN: {ISBN} already exists!");
}
