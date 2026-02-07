using API.Models.Request;
using Swashbuckle.AspNetCore.Filters;

namespace API.Swagger.Examples.Book
{
    public class StockAdjustmentExample : IExamplesProvider<StockAdjustmentRequest>
    {
        public StockAdjustmentRequest GetExamples()
        {
            return new StockAdjustmentRequest
            {
                ChangeAmount = 5
            };
        }
    }
}
