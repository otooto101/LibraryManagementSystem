
namespace Application.Models
{
    public class AuthorSearchParameters : PagedSearchParameters
    {
        public string? FullName { get; set; }
        public AuthorSortBy? SortBy { get; set; }
    }
    public enum AuthorSortBy
    {
        None,
        NameAsc,
        NameDesc,
        DobAsc,
        DobDesc
    }
}
