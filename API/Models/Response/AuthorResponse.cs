namespace API.Models.Response
{
    public class AuthorResponse : LinkedResource
    {
        public string AuthorId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateOnly? DateOfBirth { get; set; }
        public string? Biography { get; set; }
    }
}