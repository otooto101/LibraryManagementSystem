using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Extensions;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class BaseRepository<T, TContext>(TContext context) : IRepository<T>
        where T : class
        where TContext : DbContext
    {
        protected readonly TContext _context = context;

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
            => await _context.Set<T>().ToListAsync(cancellationToken);

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<T>().AddAsync(entity, cancellationToken);
            return entity;
        }

        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().AnyAsync(predicate, cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            try
            {
                return await _context.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                throw ex.TranslateDbException();
            }
        }

        public async Task<TResult> ExecuteInTransactionAsync<TResult>(
                Func<CancellationToken, Task<TResult>> operation,
                CancellationToken ct = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync(ct);
                try
                {
                    var result = await operation(ct);
                    await transaction.CommitAsync(ct);
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(ct);
                    throw;
                }
            });
        }
    }
}