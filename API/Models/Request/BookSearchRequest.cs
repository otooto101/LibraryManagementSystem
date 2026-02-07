using Application.Models;

namespace API.Models.Request
{
    public class BookSearchRequest : PagedSearchParameters
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? ISBN { get; set; }
    }
}
