using API.Models.Response;
using Application.DTOs.Book;
using Swashbuckle.AspNetCore.Filters;

namespace API.Swagger.Examples.Book
{
    public class BookResponseExample : IExamplesProvider<BookResponse>
    {
        public BookResponse GetExamples()
        {
            return new BookResponse
            {
                BookId = "hashId",
                Title = "The Hobbit",
                ISBN = "978-0547928227",
                PublicationYear = 1937,
                Quantity = 10,
                IsAvailable = true,
                Description = "A fantasy novel by J. R. R. Tolkien.",
                CoverImageUrl = "https://images.example.com/hobbit.jpg",
                Author = new AuthorBriefDto
                {
                    AuthorId = "hashId",
                    FullName = "J.R.R. Tolkien"
                },
                Links = new List<LinkDto>
                {
                    new LinkDto("https://localhost:7281/api/v1/Book/hashId", "self", "GET"),
                    new LinkDto("https://localhost:7281/api/v1/Book/hashId", "delete_book", "DELETE"),
                    new LinkDto("https://localhost:7281/api/v1/Book/hashId", "update_book", "PUT"),
                    new LinkDto("https://localhost:7281/api/v1/Book/hashId/stock", "adjust_stock", "POST")
                }
            };
        }
    }
}
