using Application.DTOs.Author;
using Swashbuckle.AspNetCore.Filters;

namespace API.Swagger.Examples.Author
{
    public class AuthorCreateExample : IExamplesProvider<AuthorCreateDto>
    {
        public AuthorCreateDto GetExamples()
        {
            return new AuthorCreateDto
            {
                FirstName = "J.R.R.",
                LastName = "Tolkien",
                DateOfBirth = new DateOnly(1892, 1, 3),
                Biography = "English writer, poet, philologist, and academic, best known as the author of the high fantasy works The Hobbit and The Lord of the Rings."
            };
        }
    }
}
