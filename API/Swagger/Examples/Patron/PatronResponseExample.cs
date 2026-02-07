using API.Models.Response;
using Swashbuckle.AspNetCore.Filters;

namespace API.Swagger.Examples.Patron
{
    public class PatronResponseExample : IExamplesProvider<PatronResponse>
    {
        public PatronResponse GetExamples()
        {
            return new PatronResponse
            {
                PatronId = "hashId",
                FirstName = "Hermione",
                LastName = "Granger",
                Email = "h.granger@hogwarts.edu",
                Links = new List<LinkDto>
                {
                    new LinkDto("https://localhost:7281/api/v1/Patrons/hashId", "self", "GET"),
                    new LinkDto("https://localhost:7281/api/v1/Patrons/hashId", "delete_patron", "DELETE"),
                    new LinkDto("https://localhost:7281/api/v1/Patrons/hashId", "update_patron", "PUT")
                }
            };
        }
    }
}
