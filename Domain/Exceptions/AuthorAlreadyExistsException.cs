namespace Domain.Exceptions
{
    public class AuthorAlreadyExistsException(string firstName, string lastName, DateOnly? dateOfBirth) 
        : Exception($"Author {firstName} {lastName} born in {dateOfBirth} already exists!");
}
