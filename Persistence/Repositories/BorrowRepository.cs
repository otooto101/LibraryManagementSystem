using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Extensions;

namespace Persistence.Repositories
{
    public class BorrowRepository(LibraryContext context) : BaseRepository<BorrowRecord, LibraryContext>(context), IBorrowRepository
    {
        public async Task<PagedResult<BorrowRecord>> GetBorrowsAsync(BorrowSearchParameters searchParams, CancellationToken ct = default)
        {
            var query = context.BorrowRecords
                .Include(b => b.Book)
                .Include(b => b.Patron)
                .AsNoTracking();

            if (searchParams.PatronId.HasValue)
                query = query.Where(b => b.PatronId == searchParams.PatronId.Value);

            if (searchParams.Status.HasValue)
                query = query.Where(b => b.Status == searchParams.Status.Value);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Page(searchParams.PageNumber - 1, searchParams.PageSize)
                .ToListAsync(ct);

            return new PagedResult<BorrowRecord>(items, totalCount, searchParams.PageNumber, searchParams.PageSize);
        }

        public async Task<IEnumerable<BorrowRecord>> GetActiveLoansForPatronAsync(int patronId, CancellationToken ct = default)
        {
            return await context.BorrowRecords
                .Include(b => b.Book)
                .Where(b => b.PatronId == patronId && b.ReturnDate == null)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<PagedResult<BorrowRecord>> GetOverdueRecordsAsync(PagedSearchParameters searchParams, CancellationToken ct = default)
        {
            var query = context.BorrowRecords
                .Include(b => b.Book)
                .Include(b => b.Patron)
                .Where(b => b.Status == BorrowStatus.Overdue)
                .AsNoTracking();

            var totalCount = await query.CountAsync(ct);
            var items = await query.Page(searchParams.PageNumber - 1, searchParams.PageSize).ToListAsync(ct);

            return new PagedResult<BorrowRecord>(items, totalCount, searchParams.PageNumber, searchParams.PageSize);
        }

        public async Task<IEnumerable<BorrowRecord>> GetActiveOverdueRecordsAsync(CancellationToken ct = default)
        {
            return await context.BorrowRecords
                .Where(b => b.ReturnDate == null &&
                            b.DueDate < DateTime.UtcNow &&
                            b.Status != BorrowStatus.Overdue)
                .ToListAsync(ct);
        }

    }
}
