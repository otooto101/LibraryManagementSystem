
namespace Domain.Exceptions
{
    public class BookNotFoundException : NotFoundException
    {
        public BookNotFoundException(string message) : base(message) { }

        public BookNotFoundException(int id)
            : base($"Book with ID {id} was not found in the library catalog.") { }
    }
}
