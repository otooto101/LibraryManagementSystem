using Application.DTOs.Book;
using Swashbuckle.AspNetCore.Filters;

namespace API.Swagger.Examples.Book
{
    public class BookCreateExample : IExamplesProvider<BookCreateDto>
    {
        public BookCreateDto GetExamples()
        {
            return new BookCreateDto
            {
                Title = "The Hobbit",
                AuthorId = "hashId",
                ISBN = "978-0547928227",
                PublicationYear = 1937,
                Quantity = 10,
                Description = "A fantasy novel by J. R. R. Tolkien.",
                CoverImageUrl = "https://images.example.com/hobbit.jpg"
            };
        }
    }
}
