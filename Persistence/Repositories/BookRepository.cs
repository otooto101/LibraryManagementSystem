using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Extensions;


namespace Persistence.Repositories
{
    public class BookRepository(LibraryContext context) : BaseRepository<Book, LibraryContext>(context), IBookRepository
    {
        public async Task<PagedResult<Book>> GetBooksAsync(BookSearchParameters searchParams, CancellationToken ct = default)
        {
            var query = context.Books
                .Include(b => b.Author)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchParams.Title))
            {
                var searchTerm = searchParams.Title.ToFtsString();

                query = query.Where(b => EF.Functions.Contains(b.Title, searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(searchParams.Author))
            {
                var searchTerm = $"\"{searchParams.Author}*\"";

                query = query.Where(b => EF.Functions.Contains(b.Author!.FullName, searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(searchParams.ISBN))
            {
                query = query.Where(b => b.ISBN == searchParams.ISBN);
            }

            if (searchParams.BorrowerId.HasValue)
            {
                query = query.Where(b => b.BorrowRecords.Any(br => br.PatronId == searchParams.BorrowerId.Value));
            }

            // Count and Paging
            var totalCount = await query.CountAsync(ct);

            query = query.Page<Book>(searchParams.PageNumber - 1, searchParams.PageSize);

            var items = await query.ToListAsync(ct);

            return new PagedResult<Book>(items, totalCount, searchParams.PageNumber, searchParams.PageSize);
        }

        public Task<Book?> GetBookWithAuthorAsync(int id, CancellationToken ct = default)
        {
            return context.Books
                .Include(b => b.Author)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookId == id);
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId, CancellationToken ct = default)
        {
            return await context.Books
                .Where(b => b.AuthorId == authorId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> TryDecrementStockAsync(int bookId, CancellationToken ct = default)
        {
            int rowsAffected = await context.Books
                .Where(b => b.BookId == bookId && b.Quantity > 0)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(b => b.Quantity, b => b.Quantity - 1),
                    ct);

            return rowsAffected > 0;
        }

        public async Task<int> AdjustStockAsync(int bookId, int changeAmount, CancellationToken ct = default)
        {
            return await context.Books
                .Where(b => b.BookId == bookId)
                .Where(b => b.Quantity + changeAmount >= 0)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(b => b.Quantity, b => b.Quantity + changeAmount),
                    ct
                );
        }
    }
}
