using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Extensions;

namespace Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryContext _context;
        public IBookRepository Books { get; }
        public IPatronRepository Patrons { get; }
        public IBorrowRepository Borrows { get; }

        public UnitOfWork(
            LibraryContext context,
            IBookRepository books,
            IPatronRepository patrons,
            IBorrowRepository borrows)
        {
            _context = context;
            Books = books;
            Patrons = patrons;
            Borrows = borrows;
        }

        public async Task CommitAsync(CancellationToken ct = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            try
            {
                await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync(ct);
                    try
                    {
                        await _context.SaveChangesAsync(ct);
                        await transaction.CommitAsync(ct);
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync(ct);
                        throw;
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex.TranslateDbException();
            }
        }

        public void Dispose() => _context.Dispose();
    }
}
