using Application.Models;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPatronRepository : IRepository<Patron>
    {
        Task<PagedResult<Patron>> GetPatronsAsync(PatronSearchParameters searchParams, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, int? excludePatronId = null, CancellationToken ct = default);
    }
}
