using Application.DTOs.Book;
using Application.Models;

namespace Application.Interfaces
{
    public interface IBookService
    {
        Task<PagedResult<BookDto>> GetPagedBooksAsync(BookSearchParameters searchParams, CancellationToken ct = default);
        Task<BookDto> GetBookByIdAsync(int id, CancellationToken ct = default);
        Task<BookDto> CreateBookAsync(BookCreateDto createDto, CancellationToken ct = default);
        Task UpdateBookAsync(int id, BookUpdateDto updateDto, CancellationToken ct = default);
        Task<bool> CheckAvailabilityAsync(int id, CancellationToken ct = default);
        Task DeleteBookAsync(int id, CancellationToken ct = default);
        Task UpdateStockAsync(int id, int changeAmount, CancellationToken ct = default);
    }
}
