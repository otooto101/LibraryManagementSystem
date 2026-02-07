using Application.DTOs.Borrowing;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Exceptions;
using Mapster;

namespace Application.Services
{
    public class BorrowService(IUnitOfWork unitOfWork, IHashIdService hashIdService) : IBorrowService
    {
        public async Task<PagedResult<BorrowDto>> GetPagedBorrowsAsync(BorrowSearchParameters searchParams, CancellationToken ct = default)
        {
            var pagedRecords = await unitOfWork.Borrows.GetBorrowsAsync(searchParams, ct);
            var dtos = pagedRecords.Items.Adapt<IEnumerable<BorrowDto>>();
            return new PagedResult<BorrowDto>(dtos.ToList(), pagedRecords.TotalCount, pagedRecords.PageNumber, pagedRecords.PageSize);
        }

        public async Task<BorrowDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var record = await unitOfWork.Borrows.GetByIdAsync(id, ct);
            if (record == null) throw new BorrowRecordNotFoundException(id);
            return record.Adapt<BorrowDto>();
        }

        public async Task<BorrowDto> CheckoutBookAsync(BorrowBookDto dto, CancellationToken ct = default)
        {
            int internalBookId = hashIdService.Decode(dto.BookId);
            int internalPatronId = hashIdService.Decode(dto.PatronId);

            var book = await unitOfWork.Books.GetByIdAsync(internalBookId, ct);
            if (book == null) throw new BookNotFoundException(dto.BookId);

            var patron = await unitOfWork.Patrons.GetByIdAsync(internalBookId, ct);
            if (patron == null) throw new PatronNotFoundException(dto.PatronId);

            bool decrementSuccess = await unitOfWork.Books.TryDecrementStockAsync(internalBookId, ct);
            if (!decrementSuccess) throw new BookOutOfStockException(book.Title);

            var borrowRecord = new BorrowRecord(internalBookId, internalPatronId, dto.DaysAllowed);

            borrowRecord.Book = book;
            borrowRecord.Patron = patron;

            await unitOfWork.Borrows.AddAsync(borrowRecord, ct);

            try
            {
                await unitOfWork.CommitAsync(ct);
            }
            catch (ConcurrencyException ex)
            {
                throw new DomainRuleViolationException("Transaction failed due to concurrent modification.", ex);
            }

            return borrowRecord.Adapt<BorrowDto>();
        }

        public async Task ReturnAsync(int borrowRecordId, CancellationToken ct = default)
        {
            var record = await unitOfWork.Borrows.GetByIdAsync(borrowRecordId, ct);
            if (record == null) throw new BorrowRecordNotFoundException(borrowRecordId);

            if (record.ReturnDate.HasValue)
                throw new BookAlreadyReturnedException("This book has already been returned.");

            record.MarkAsReturned();
            await unitOfWork.Borrows.UpdateAsync(record, ct);

            var rowsAffected = await unitOfWork.Books.AdjustStockAsync(record.BookId, 1, ct);
            if (rowsAffected == 0) throw new BookNotFoundException(record.BookId);

            try
            {
                await unitOfWork.CommitAsync(ct);
            }
            catch (ConcurrencyException)
            {
                throw new BookAlreadyReturnedException("Record was modified by another user.");
            }
        }

        public async Task<PagedResult<BorrowDto>> GetOverdueRecordsAsync(PagedSearchParameters searchParams, CancellationToken ct = default)
        {
            var pagedRecords = await unitOfWork.Borrows.GetOverdueRecordsAsync(searchParams, ct);
            var dtos = pagedRecords.Items.Adapt<IEnumerable<BorrowDto>>();

            return new PagedResult<BorrowDto>(dtos.ToList(), pagedRecords.TotalCount, pagedRecords.PageNumber, pagedRecords.PageSize);
        }

        public async Task DeleteRecordAsync(int id, CancellationToken ct = default)
        {
            var record = await unitOfWork.Borrows.GetByIdAsync(id, ct);
            if (record == null) throw new BorrowRecordNotFoundException(id);

            if (record.Status != BorrowStatus.Returned)
            {
                await unitOfWork.Books.AdjustStockAsync(record.BookId, 1, ct);
            }

            await unitOfWork.Borrows.DeleteAsync(record, ct);
            await unitOfWork.CommitAsync(ct);
        }

    }
}
