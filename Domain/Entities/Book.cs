
namespace Domain.Entities
{
    public class Book
    {
        public int BookId { get; set; }
        public required string Title { get; set; }
        public string? ISBN { get; set; }
        public required int PublicationYear { get; set; }
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public int Quantity { get; set; }
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();

    }
}
