using Application.Models;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<PagedResult<Book>> GetBooksAsync(BookSearchParameters searchParams, CancellationToken ct = default);
        Task<Book?> GetBookWithAuthorAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId, CancellationToken ct = default);
        Task<bool> TryDecrementStockAsync(int bookId, CancellationToken ct = default);
        Task<int> AdjustStockAsync(int bookId, int changeAmount, CancellationToken ct = default);
    }
}
