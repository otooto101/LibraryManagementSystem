using System.Net;
using Domain.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace API.Middlewares.Configuration
{

    public static class ExceptionConfiguration
    {
        private static readonly Dictionary<Type, HttpStatusCode> _exceptionMap = new()
        {
        // 404 Not Found
        { typeof(NotFoundException), HttpStatusCode.NotFound },
        { typeof(BookNotFoundException), HttpStatusCode.NotFound },
        { typeof(PatronNotFoundException), HttpStatusCode.NotFound },
        { typeof(BorrowRecordNotFoundException), HttpStatusCode.NotFound },

        // 409 Conflict (Duplicates / Concurrency)
        { typeof(AuthorAlreadyExistsException), HttpStatusCode.Conflict },
        { typeof(BookAlreadyExistsException), HttpStatusCode.Conflict },
        { typeof(EmailAlreadyExistsException), HttpStatusCode.Conflict },
        { typeof(ConcurrencyException), HttpStatusCode.Conflict },
        { typeof(DuplicateResourceException), HttpStatusCode.Conflict },

        // 400 Bad Request (Business Rule Violations)
        { typeof(BookOutOfStockException), HttpStatusCode.BadRequest },
        { typeof(InsufficientStockException), HttpStatusCode.BadRequest },
        { typeof(BookAlreadyReturnedException), HttpStatusCode.BadRequest },
        { typeof(CannotDeleteAuthorException), HttpStatusCode.BadRequest },
        { typeof(CannotDeleteBookException), HttpStatusCode.BadRequest },
        { typeof(PatronHasActiveLoansException), HttpStatusCode.BadRequest },
        { typeof(DomainRuleViolationException), HttpStatusCode.BadRequest },

        { typeof(UnauthorizedAccessException), HttpStatusCode.Unauthorized },
        { typeof(SecurityTokenException), HttpStatusCode.Unauthorized },
        { typeof(SecurityTokenExpiredException), HttpStatusCode.Unauthorized }
        };

        public static HttpStatusCode GetStatusCode(Exception ex)
        {
            var type = ex.GetType();

            if (_exceptionMap.TryGetValue(type, out var statusCode))
                return statusCode;

            // Inheritance Lookup (If the specific type isn't mapped, check the base)
            return ex switch
            {
                NotFoundException => HttpStatusCode.NotFound,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };
        }
    }
}
