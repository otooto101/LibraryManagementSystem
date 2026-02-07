
namespace Application.DTOs.Author
{
    public class AuthorDto
    {
        public string AuthorId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateOnly? DateOfBirth { get; set; }
        public string? Biography { get; set; }
    }

}
