namespace API.Models.Response
{
    public class PatronResponse : LinkedResource
    {
        public string PatronId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime MembershipDate { get; set; }
    }
}