using Domain.Entities;

namespace Application.DTOs.Borrowing
{
    public class BorrowDto
    {
        public string Id { get; set; }
        public string PatronId { get; set; }
        public string PatronName { get; set; } = null!;
        public string BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public BorrowStatus Status { get; set; }
    }
}
