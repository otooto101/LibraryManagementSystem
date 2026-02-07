using Application.DTOs.Author;
using Swashbuckle.AspNetCore.Filters;

namespace API.Swagger.Examples.Author
{
    public class AuthorUpdateExample : IExamplesProvider<AuthorUpdateDto>
    {
        public AuthorUpdateDto GetExamples()
        {
            return new AuthorUpdateDto
            {
                AuthorId = "hashId",
                FirstName = "John Ronald Reuel",
                LastName = "Tolkien",
                DateOfBirth = new DateOnly(1892, 1, 3),
                Biography = "Updated biography with more details about his academic career at Oxford."
            };
        }
    }
}
