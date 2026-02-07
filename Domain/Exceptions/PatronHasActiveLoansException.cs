namespace Domain.Exceptions
{
    public class PatronHasActiveLoansException(int patronId) 
        : Exception($"Cannot delete Patron {patronId} because they still have unreturned books.");
}
