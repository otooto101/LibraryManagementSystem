
namespace Domain.Entities
{
    public class Patron
    {
        public int PatronId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public DateTime MembershipDate { get; private set; }
        public virtual ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();

        public Patron()
        {
            MembershipDate = DateTime.UtcNow;
        }
    }
}
