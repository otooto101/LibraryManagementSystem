using Application.DTOs.Patron;
using Swashbuckle.AspNetCore.Filters;

namespace API.Swagger.Examples.Patron
{
    public class PatronUpdateExample : IExamplesProvider<PatronUpdateDto>
    {
        public PatronUpdateDto GetExamples()
        {
            return new PatronUpdateDto
            {
                FirstName = "Hermione",
                LastName = "Weasley",
                Email = "h.weasley@hogwarts.edu"
            };
        }
    }
}
