
namespace Application.DTOs.Borrowing
{
    public class BorrowBookDto
    {
        public string PatronId { get; set; }
        public string BookId { get; set; }
        public int DaysAllowed { get; set; } = 14;
    }
}
