using Application.Models;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IBorrowRepository : IRepository<BorrowRecord>
    {
        Task<PagedResult<BorrowRecord>> GetBorrowsAsync(BorrowSearchParameters searchParams, CancellationToken ct = default);
        Task<IEnumerable<BorrowRecord>> GetActiveLoansForPatronAsync(int patronId, CancellationToken ct = default);
        Task<PagedResult<BorrowRecord>> GetOverdueRecordsAsync(PagedSearchParameters searchParams, CancellationToken ct = default);
        Task<IEnumerable<BorrowRecord>> GetActiveOverdueRecordsAsync(CancellationToken ct = default);
    }
}
