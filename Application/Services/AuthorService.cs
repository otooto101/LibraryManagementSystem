using Application.DTOs.Author;
using Application.DTOs.Book;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Exceptions;
using Mapster;

namespace Application.Services
{
    public class AuthorService(IAuthorRepository authorRepository, IBookRepository bookRepository, IHashIdService hashIdService) : IAuthorService
    {
        public async Task<PagedResult<AuthorDto>> GetPagedAuthorsAsync(AuthorSearchParameters searchParams, CancellationToken ct = default)
        {
            var pagedAuthors = await authorRepository.GetAuthorsAsync(searchParams, ct);
            return pagedAuthors.Adapt<PagedResult<AuthorDto>>();
        }

        public async Task<AuthorDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var author = await authorRepository.GetByIdAsync(id, ct);
            if (author == null) throw new AuthorNotFoundException(id);
            return author.Adapt<AuthorDto>();
        }

        public async Task<AuthorDto> CreateAuthorAsync(AuthorCreateDto createDto, CancellationToken ct = default)
        {
            var exists = await authorRepository.AnyAsync(a =>
                a.FirstName == createDto.FirstName &&
                a.LastName == createDto.LastName &&
                a.DateOfBirth == createDto.DateOfBirth, ct);

            if (exists)
            {
                throw new AuthorAlreadyExistsException(createDto.FirstName, createDto.LastName, createDto.DateOfBirth);
            }

            var author = createDto.Adapt<Author>();
            await authorRepository.AddAsync(author, ct);

            // i did this cuz what if 2 person insert same time ? and this exists couldn't identify that insert
            // then exception will occur and here it is handling of that case 
            try
            {
                await authorRepository.SaveChangesAsync(ct);
            }
            catch (DuplicateResourceException) // <--- Very Clean!
            {
                throw new AuthorAlreadyExistsException(createDto.FirstName, createDto.LastName, createDto.DateOfBirth);
            }

            return author.Adapt<AuthorDto>();
        }

        public async Task UpdateAuthorAsync(AuthorUpdateDto updateDto, CancellationToken ct = default)
        {
            int internalId = hashIdService.Decode(updateDto.AuthorId);

            var existingAuthor = await authorRepository.GetByIdAsync(internalId, ct);

            if (existingAuthor == null)
                throw new AuthorNotFoundException(updateDto.AuthorId);

            updateDto.Adapt(existingAuthor);

            await authorRepository.UpdateAsync(existingAuthor, ct);

            try
            {
                await authorRepository.SaveChangesAsync(ct);
            }
            catch (ConcurrencyException ex)
            {
                throw new ConcurrencyException(
                    $"The author '{existingAuthor.FullName}' was removed by someone else while you were editing.",
                    ex);
            }
        }

        public async Task DeleteAuthorAsync(int id, CancellationToken ct = default)
        {
            var author = await authorRepository.GetByIdAsync(id, ct);
            if (author == null) throw new AuthorNotFoundException(id);

            var hasBooks = await bookRepository.AnyAsync(b => b.AuthorId == id, ct);
            if (hasBooks)
            {
                throw new CannotDeleteAuthorException(id);
            }

            await authorRepository.DeleteAsync(author, ct);

            try
            {
                await authorRepository.SaveChangesAsync(ct);
            }
            catch (DomainRuleViolationException) // what if someone added book for this author when i was deleting author?
            {
                throw new CannotDeleteAuthorException(id);
            }
            catch (ConcurrencyException)
            {
                // what if someoene deleted it before i delete in same time? this catch handles that
                throw new AuthorNotFoundException(id);
            }
        }

        public async Task<IEnumerable<BookDto>> GetBooksByAuthorAsync(int authorId, CancellationToken ct = default)
        {
            var exists = await authorRepository.AnyAsync(a => a.AuthorId == authorId, ct);

            if (!exists) throw new AuthorNotFoundException(authorId);

            var books = await bookRepository.GetBooksByAuthorIdAsync(authorId, ct);
            return books.Adapt<IEnumerable<BookDto>>();
        }
    }
}
