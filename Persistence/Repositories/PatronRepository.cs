using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Extensions;

namespace Persistence.Repositories
{
    public class PatronRepository(LibraryContext context) : BaseRepository<Patron, LibraryContext>(context), IPatronRepository
    {
        public async Task<PagedResult<Patron>> GetPatronsAsync(PatronSearchParameters searchParams, CancellationToken ct = default)
        {
            var query = context.Patrons.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var term = searchParams.SearchTerm.Trim();

                query = query.Where(p => p.FirstName.StartsWith(term) ||
                                                 p.LastName.StartsWith(term) ||
                                                 (p.Email.StartsWith(term)));
            }

            var totalCount = await query.CountAsync(ct);

            query = query.OrderBy(p => p.LastName).ThenBy(p => p.FirstName);

            query = query.Page(searchParams.PageNumber - 1, searchParams.PageSize);

            var items = await query.ToListAsync(ct);

            return new PagedResult<Patron>(items, totalCount, searchParams.PageNumber, searchParams.PageSize);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludePatronId = null, CancellationToken ct = default)
        {
            var query = context.Patrons.AsNoTracking();

            if (excludePatronId.HasValue)
            {
                return await query.AnyAsync(p => p.Email == email && p.PatronId != excludePatronId.Value, ct);
            }

            return await query.AnyAsync(p => p.Email == email, ct);
        }
    }
}