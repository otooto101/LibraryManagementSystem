namespace Application.DTOs.Book
{
    public class BookCreateDto
    {
        public string Title { get; set; } = null!;
        public string AuthorId { get; set; }
        public string ISBN { get; set; } = null!;
        public int PublicationYear { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
    }
}
