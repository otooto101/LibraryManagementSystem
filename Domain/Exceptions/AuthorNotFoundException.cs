namespace Domain.Exceptions
{
    public class AuthorNotFoundException : NotFoundException
    {
        public AuthorNotFoundException(string message) : base(message) { }

        public AuthorNotFoundException(int id)
            : base($"Author with ID {id} not found.") { }
    }
}
