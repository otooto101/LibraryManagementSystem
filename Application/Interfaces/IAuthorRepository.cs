using Application.Models;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAuthorRepository : IRepository<Author>
    {
        Task<PagedResult<Author>> GetAuthorsAsync(AuthorSearchParameters filters, CancellationToken ct = default);
    }
}
