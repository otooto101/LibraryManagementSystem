using Application.DTOs.Patron;
using Domain.Exceptions;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Mapster;

namespace Application.Services
{
    public class PatronService(IPatronRepository patronRepository, IBorrowRepository borrowRepository) : IPatronService
    {
        public async Task<PagedResult<PatronDto>> GetPagedPatronsAsync(PatronSearchParameters searchParams, CancellationToken ct = default)
        {
            var pagedEntity = await patronRepository.GetPatronsAsync(searchParams, ct);

            var dtos = pagedEntity.Items.Adapt<IEnumerable<PatronDto>>();

            return new PagedResult<PatronDto>(dtos.ToList(), pagedEntity.TotalCount, pagedEntity.PageNumber, pagedEntity.PageSize);
        }

        public async Task<PatronDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var patron = await patronRepository.GetByIdAsync(id, ct);
            if (patron == null) throw new PatronNotFoundException(id);

            return patron.Adapt<PatronDto>();
        }

        public async Task<PatronDto> CreatePatronAsync(PatronCreateDto createDto, CancellationToken ct = default)
        {
            if (!string.IsNullOrWhiteSpace(createDto.Email))
            {
                bool isTaken = await patronRepository.EmailExistsAsync(createDto.Email, null, ct);
                if (isTaken)
                {
                    throw new EmailAlreadyExistsException(createDto.Email);
                }
            }

            var patron = createDto.Adapt<Patron>();
            await patronRepository.AddAsync(patron, ct);

            try
            {
                await patronRepository.SaveChangesAsync(ct);
            }
            catch (DuplicateResourceException)
            {
                throw new EmailAlreadyExistsException(createDto.Email);
            }

            return patron.Adapt<PatronDto>();
        }

        public async Task UpdatePatronAsync(int id, PatronUpdateDto updateDto, CancellationToken ct = default)
        {
            var existingPatron = await patronRepository.GetByIdAsync(id, ct);
            if (existingPatron == null) throw new PatronNotFoundException(id);

            if (!string.IsNullOrWhiteSpace(updateDto.Email) && updateDto.Email != existingPatron.Email)
            {
                bool isTaken = await patronRepository.EmailExistsAsync(updateDto.Email, id, ct);
                if (isTaken) throw new EmailAlreadyExistsException(updateDto.Email);
            }

            updateDto.Adapt(existingPatron);
            await patronRepository.UpdateAsync(existingPatron, ct);

            try
            {
                await patronRepository.SaveChangesAsync(ct);
            }
            catch (ConcurrencyException ex)
            {
                // Case: Someone else deleted this patron while we were editing
                throw new ConcurrencyException(
                    $"The patron '{existingPatron.FirstName} {existingPatron.LastName}' was removed by another user. Please refresh and try again.",
                    ex);
            }
            catch (DuplicateResourceException)
            {
                // Case: Race condition - Two users grabbed the same email at the exact same millisecond
                throw new EmailAlreadyExistsException(updateDto.Email ?? "Unknown");
            }
        }

        public async Task DeletePatronAsync(int id, CancellationToken ct = default)
        {
            var patron = await patronRepository.GetByIdAsync(id, ct);
            if (patron == null) throw new PatronNotFoundException(id);


            var hasLoans = await borrowRepository
                    .AnyAsync(b => b.PatronId == id && b.ReturnDate == null, ct);

            if (hasLoans)
            {
                throw new PatronHasActiveLoansException(id);
            }

            await patronRepository.DeleteAsync(patron, ct);

            try
            {
                await patronRepository.SaveChangesAsync(ct);
            }
            catch (DomainRuleViolationException)
            {
                // (e.g. a loan was created at the last millisecond)
                throw new PatronHasActiveLoansException(id);
            }
            catch (ConcurrencyException)
            {
                // This happens if the Patron was already deleted by someone else
                throw new PatronNotFoundException(id);
            }
        }
    }
}