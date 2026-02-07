namespace Domain.Exceptions
{
    public class BookOutOfStockException(string bookTitle) : Exception($"Book '${bookTitle}' is out of stock.");
}
