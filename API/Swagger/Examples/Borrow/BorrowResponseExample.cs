using API.Models.Response;
using Domain.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace API.Swagger.Examples.Borrow
{
    public class BorrowResponseExample : IExamplesProvider<BorrowResponse>
    {
        public BorrowResponse GetExamples()
        {
            return new BorrowResponse
            {
                Id = "hashId",
                PatronId = "hashId",
                PatronName = "John Doe",
                BookId = "hashId",
                BookTitle = "The Hobbit",
                BorrowDate = DateTime.Now.AddDays(-5),
                DueDate = DateTime.Now.AddDays(9),
                ReturnDate = null,
                Status = BorrowStatus.Borrowed,
                Links = new List<LinkDto>
                {
                    new LinkDto("https://localhost:7281/api/v1/BorrowRecords/hashId", "self", "GET"),
                    new LinkDto("https://localhost:7281/api/v1/BorrowRecords/hashId/return", "return_book", "PUT")
                }
            };
        }
    }
}
