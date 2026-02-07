namespace Domain.Exceptions
{
    public class EmailAlreadyExistsException(string email) : Exception($"The email '{email}' is already in use.");
}
