using Application.DTOs.Borrowing;
using Application.Models;

namespace Application.Interfaces
{
    public interface IBorrowService
    {
        Task<PagedResult<BorrowDto>> GetPagedBorrowsAsync(BorrowSearchParameters searchParams, CancellationToken ct = default);
        Task<BorrowDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task<BorrowDto> CheckoutBookAsync(BorrowBookDto dto, CancellationToken ct = default);
        Task ReturnAsync(int borrowRecordId, CancellationToken ct = default);
        Task<PagedResult<BorrowDto>> GetOverdueRecordsAsync(PagedSearchParameters searchParams, CancellationToken ct = default);
        Task DeleteRecordAsync(int id, CancellationToken ct = default);
    }
}
