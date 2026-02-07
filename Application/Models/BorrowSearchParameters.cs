using Domain.Entities;

namespace Application.Models
{
    public class BorrowSearchParameters : PagedSearchParameters
    {
        public int? PatronId { get; set; }
        public int? BookId { get; set; }
        public BorrowStatus? Status { get; set; }
    }
}
