namespace Domain.Exceptions
{
    public class BorrowRecordNotFoundException(int id)
            : NotFoundException($"The borrow record with transaction ID {id} was not found.");
}
