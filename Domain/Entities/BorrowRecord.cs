
namespace Domain.Entities
{
    public class BorrowRecord
    {
        public int Id { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public BorrowStatus Status { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public int PatronId { get; set; }
        public Patron Patron { get; set; } = null!;
        protected BorrowRecord() { }
        public BorrowRecord(int bookId, int patronId, int daysAllowed)
        {
            BookId = bookId;
            PatronId = patronId;
            BorrowDate = DateTime.UtcNow;
            DueDate = BorrowDate.AddDays(daysAllowed);
            Status = BorrowStatus.Borrowed;
        }

        public void MarkAsReturned()
        {
            ReturnDate = DateTime.UtcNow;
            Status = BorrowStatus.Returned;
        }

        public void CheckIfOverdue()
        {
            if (Status == BorrowStatus.Borrowed && DateTime.UtcNow > DueDate)
            {
                Status = BorrowStatus.Overdue;
            }
        }
    }

    public enum BorrowStatus
    {
        Borrowed,
        Returned,
        Overdue
    }


}
