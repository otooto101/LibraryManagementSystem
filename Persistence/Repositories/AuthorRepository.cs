using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Extensions;

namespace Persistence.Repositories
{
    public class AuthorRepository(LibraryContext context) : BaseRepository<Author, LibraryContext>(context), IAuthorRepository
    {
        public async Task<PagedResult<Author>> GetAuthorsAsync(AuthorSearchParameters searchParams, CancellationToken ct = default)
        {
            var query = context.Authors.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchParams.FullName))
            {
                var searchTerm = searchParams.FullName.ToFtsString();

                query = query.Where(a => EF.Functions.Contains(a.FullName, searchTerm));
            }

            var totalCount = await query.CountAsync(ct);

            var sortOption = searchParams.SortBy ?? AuthorSortBy.None;
            
            query = sortOption switch
            {
                AuthorSortBy.NameAsc => query.OrderBy(a => a.FullName),
                AuthorSortBy.NameDesc => query.OrderByDescending(a => a.FullName),
                AuthorSortBy.DobAsc => query.OrderBy(a => a.DateOfBirth),
                AuthorSortBy.DobDesc => query.OrderByDescending(a => a.DateOfBirth),
                _ => query.OrderBy(a => a.AuthorId)
            };

            query = query.Page(searchParams.PageNumber - 1, searchParams.PageSize);

            var items = await query.ToListAsync(ct);

            return new PagedResult<Author>(items, totalCount, searchParams.PageNumber, searchParams.PageSize);
        }
    }
}
