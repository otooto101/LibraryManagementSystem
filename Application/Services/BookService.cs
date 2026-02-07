using Application.DTOs.Book;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Exceptions;
using Mapster;

namespace Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }
        public async Task<PagedResult<BookDto>> GetPagedBooksAsync(BookSearchParameters searchParams, CancellationToken ct = default)
        {
            var pagedBooks = await _bookRepository.GetBooksAsync(searchParams, ct);

            var dtos = pagedBooks.Items.Adapt<List<BookDto>>();

            return new PagedResult<BookDto>(dtos, pagedBooks.TotalCount, pagedBooks.PageNumber, pagedBooks.PageSize);
        }

        public async Task<BookDto> GetBookByIdAsync(int id, CancellationToken ct = default)
        {
            var book = await _bookRepository.GetBookWithAuthorAsync(id, ct);

            if (book == null) throw new BookNotFoundException(id);

            return book.Adapt<BookDto>();
        }

        public async Task<BookDto> CreateBookAsync(BookCreateDto createDto, CancellationToken ct = default)
        {
            var book = createDto.Adapt<Book>();

            await _bookRepository.AddAsync(book, ct);

            try
            {
                await _bookRepository.SaveChangesAsync(ct);
            }
            catch (DomainRuleViolationException)
            {
                // It means the AuthorId provided doesn't exist in the database, what if someone deleted on this time?
                throw new AuthorNotFoundException(createDto.AuthorId);
            }
            catch (DuplicateResourceException)
            {
                throw new BookAlreadyExistsException(createDto.ISBN);
            }

            return book.Adapt<BookDto>();
        }

        public async Task<bool> CheckAvailabilityAsync(int id, CancellationToken ct = default)
        {
            return await _bookRepository.AnyAsync(b => b.BookId == id && b.Quantity > 0, ct);
        }

        public async Task DeleteBookAsync(int id, CancellationToken ct = default)
        {
            var book = await _bookRepository.GetByIdAsync(id, ct);
            if (book == null) throw new BookNotFoundException(id);

            await _bookRepository.DeleteAsync(book, ct);

            try
            {
                await _bookRepository.SaveChangesAsync(ct);
            }
            catch (ConcurrencyException)
            {
                // Someone else deleted it
                throw new BookNotFoundException($"The book '{book.Title}' was already removed by another user.");
            }
            catch (DomainRuleViolationException)
            {
                throw new CannotDeleteBookException($"Cannot delete '{book.Title}' because it is currently borrowed.");
            }
        }

        public async Task UpdateBookAsync(int id, BookUpdateDto updateDto, CancellationToken ct = default)
        {
            var existingBook = await _bookRepository.GetByIdAsync(id, ct);
            if (existingBook == null)
                throw new BookNotFoundException(id);

            updateDto.Adapt(existingBook);
            await _bookRepository.UpdateAsync(existingBook, ct);

            try
            {
                await _bookRepository.SaveChangesAsync(ct);
            }
            catch (ConcurrencyException ex)
            {
                // Case: Someone else deleted this book since we fetched it
                throw new ConcurrencyException(
                    $"The book '{existingBook.Title}' was removed by another user. Please refresh and try again.",
                    ex);
            }
            catch (DomainRuleViolationException)
            {
                throw new AuthorNotFoundException("Author doesn't exist! maybe it was deleted by someone else!");
            }
        }

        public async Task UpdateStockAsync(int id, int changeAmount, CancellationToken ct = default)
        {
            int rowsAffected = await _bookRepository.AdjustStockAsync(id, changeAmount, ct);

            if (rowsAffected > 0)
            {
                return;
            }
            bool bookExists = await _bookRepository.AnyAsync(b => b.BookId == id, ct);

            if (!bookExists)
            {
                throw new BookNotFoundException(id);
            }
            else
            {
                // Book exists, so the failure was due to the Quantity check ( < 0 )
                throw new InsufficientStockException("Book is out of stock!");
            }
        }
    }
}
