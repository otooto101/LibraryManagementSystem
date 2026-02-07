namespace Domain.Entities
{
    public class Author
    {
        public int AuthorId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string FullName { get; private set; }
        public string? Biography { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public ICollection<Book>? Books { get; set; }
    }
}
