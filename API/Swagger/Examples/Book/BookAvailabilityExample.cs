using Application.DTOs.Book;
using Swashbuckle.AspNetCore.Filters;

namespace API.Swagger.Examples.Book
{
    public class BookAvailabilityExample : IExamplesProvider<BookAvailabilityDto>
    {
        public BookAvailabilityDto GetExamples()
        {
            return new BookAvailabilityDto
            {
                IsAvailable = true
            };
        }
    }
}
