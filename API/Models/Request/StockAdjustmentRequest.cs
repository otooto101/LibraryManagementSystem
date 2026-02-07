using Application.ValidationAttributes;
using Microsoft.AspNetCore.Mvc;

namespace API.Models.Request
{
    public class StockAdjustmentRequest
    {
        [FromBody]
        [NotZero(ErrorMessage = "Change amount cannot be zero.")]
        public int ChangeAmount { get; set; }
    }
}
