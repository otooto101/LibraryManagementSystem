using API.Models.Response;
using Swashbuckle.AspNetCore.Filters;

namespace API.Swagger.Examples.Author
{
    public class AuthorResponseExample : IExamplesProvider<AuthorResponse>
    {
        public AuthorResponse GetExamples()
        {
            return new AuthorResponse
            {
                AuthorId = "hashId",
                FirstName = "J.R.R.",
                LastName = "Tolkien",
                DateOfBirth = new DateOnly(1892, 1, 3),
                Biography = "English writer and philologist.",

                Links = new List<LinkDto>
                {
                    new LinkDto("https://localhost:7281/api/v1/Author/hashId", "self", "GET"),
                    new LinkDto("https://localhost:7281/api/v1/Author/hashId", "delete_author", "DELETE"),
                    new LinkDto("https://localhost:7281/api/v1/Author/hashId", "update_author", "PUT")
                }
            };
        }
    }
}
