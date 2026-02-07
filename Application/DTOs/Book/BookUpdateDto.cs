namespace Application.DTOs.Book
{
    public class BookUpdateDto
    {
        public string? Title { get; set; } = null!;
        public string? ISBN { get; set; }
        public int? PublicationYear { get; set; }
        public string? AuthorId { get; set; }
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
    }
}
