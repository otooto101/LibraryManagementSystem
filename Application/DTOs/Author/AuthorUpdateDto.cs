

namespace Application.DTOs.Author
{
    public class AuthorUpdateDto
    {
        public string AuthorId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Biography { get; set; }
    }
}
