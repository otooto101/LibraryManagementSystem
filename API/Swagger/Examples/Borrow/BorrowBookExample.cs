using Application.DTOs.Borrowing;
using Swashbuckle.AspNetCore.Filters;

namespace API.Swagger.Examples.Borrow
{
    public class BorrowBookExample : IExamplesProvider<BorrowBookDto>
    {
        public BorrowBookDto GetExamples()
        {
            return new BorrowBookDto
            {
                PatronId = "hashId",
                BookId = "hashId",
                DaysAllowed = 14
            };
        }
    }
}
