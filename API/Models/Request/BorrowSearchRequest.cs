using Application.Models;
using Domain.Entities;

namespace API.Models.Request
{
    public class BorrowSearchRequest : PagedSearchParameters
    {
        public string? PatronId { get; set; }
        public string? BookId { get; set; }
        public BorrowStatus? Status { get; set; }
    }
}
