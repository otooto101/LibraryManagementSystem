using Application.DTOs.Book;
using Swashbuckle.AspNetCore.Filters;

namespace API.Swagger.Examples.Book
{
    public class BookUpdateExample : IExamplesProvider<BookUpdateDto>
    {
        public BookUpdateDto GetExamples()
        {
            return new BookUpdateDto
            {
                Title = "The Hobbit: 75th Anniversary Edition",
                Description = "Updated description for the special edition.",
                PublicationYear = 2012,
                CoverImageUrl = "https://images.example.com/hobbit-75.jpg"
            };
        }
    }
}
