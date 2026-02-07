using Application.DTOs.Patron;
using Application.Models;

namespace Application.Interfaces
{
    public interface IPatronService
    {
        Task<PagedResult<PatronDto>> GetPagedPatronsAsync(PatronSearchParameters searchParams, CancellationToken ct = default);
        Task<PatronDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PatronDto> CreatePatronAsync(PatronCreateDto createDto, CancellationToken ct = default);
        Task UpdatePatronAsync(int id, PatronUpdateDto updateDto, CancellationToken ct = default);
        Task DeletePatronAsync(int id, CancellationToken ct = default);
    }
}
