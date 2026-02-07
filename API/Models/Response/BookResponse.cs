using Application.DTOs.Book;
namespace API.Models.Response
{
    public class BookResponse : LinkedResource
    {
        public string BookId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public int PublicationYear { get; set; }
        public int Quantity { get; set; }
        public bool IsAvailable { get; set; }
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public AuthorBriefDto Author { get; set; } = null!;
    }
}