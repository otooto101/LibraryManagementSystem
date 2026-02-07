namespace Domain.Exceptions
{
    public class PatronNotFoundException : NotFoundException
    {
        public PatronNotFoundException(string message) : base(message) { }

        public PatronNotFoundException(int id)
            : base($"Patron with ID {id} not found.") { }
    }
}
