using Application.DTOs.Patron;
using Swashbuckle.AspNetCore.Filters;

namespace API.Swagger.Examples.Patron
{
    public class PatronCreateExample : IExamplesProvider<PatronCreateDto>
    {
        public PatronCreateDto GetExamples()
        {
            return new PatronCreateDto
            {
                FirstName = "Hermione",
                LastName = "Granger",
                Email = "h.granger@hogwarts.edu"
            };
        }
    }
}
