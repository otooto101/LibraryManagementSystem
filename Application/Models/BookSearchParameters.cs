
namespace Application.Models
{
    public class BookSearchParameters : PagedSearchParameters
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? ISBN { get; set; }
        public int? BorrowerId { get; set; }
    }
}
