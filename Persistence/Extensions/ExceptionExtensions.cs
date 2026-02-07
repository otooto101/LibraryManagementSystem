using Domain.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace Persistence.Extensions
{
    public static class ExceptionExtensions
    {
        public static Exception TranslateDbException(this Exception ex)
        {
            if (ex is ConcurrencyException || ex is DomainRuleViolationException)
                return ex;

            if (ex is DbUpdateConcurrencyException concurrencyEx)
            {
                return new ConcurrencyException("A concurrency conflict occurred.", concurrencyEx);
            }

            if (ex is DbUpdateException dbEx && dbEx.InnerException is SqlException sqlEx)
            {
                return sqlEx.Number switch
                {
                    2601 or 2627 => new DuplicateResourceException("Duplicate resource detected.", dbEx),
                    547 => new DomainRuleViolationException("A business rule prevents this change.", dbEx),
                    _ => ex
                };
            }

            return ex;
        }
    }
}
