
namespace Application.DTOs.Patron
{
    public class PatronDto
    {
        public string PatronId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime MembershipDate { get; set; }
        public string? Email { get; set; }
    }
}
