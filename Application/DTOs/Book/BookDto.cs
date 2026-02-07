
namespace Application.DTOs.Book
{
    public class BookDto
    {
        public string BookId { get; set; }
        public string Title { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public int PublicationYear { get; set; }
        public int Quantity { get; set; }
        public bool IsAvailable { get; set; }

        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }

        public AuthorBriefDto Author { get; set; } = null!;
    }

    public class AuthorBriefDto
    {
        public string AuthorId { get; set; }
        public string FullName { get; set; } = null!;
    }
}
