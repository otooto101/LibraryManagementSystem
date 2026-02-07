using Application.DTOs.Author;
using Application.DTOs.Book;
using Application.Models;

namespace Application.Interfaces
{
    public interface IAuthorService
    {
        Task<PagedResult<AuthorDto>> GetPagedAuthorsAsync(AuthorSearchParameters searchParams, CancellationToken ct = default);
        Task<AuthorDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task<AuthorDto> CreateAuthorAsync(AuthorCreateDto createDto, CancellationToken ct = default);
        Task UpdateAuthorAsync(AuthorUpdateDto updateDto, CancellationToken ct = default);
        Task DeleteAuthorAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<BookDto>> GetBooksByAuthorAsync(int authorId, CancellationToken ct = default);
    }
}
