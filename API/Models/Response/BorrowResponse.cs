using Domain.Entities;

namespace API.Models.Response
{
    public class BorrowResponse : LinkedResource
    {
        public string Id { get; set; } = null!;
        public string PatronId { get; set; } = null!;
        public string PatronName { get; set; } = null!;
        public string BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public BorrowStatus Status { get; set; }
    }
}