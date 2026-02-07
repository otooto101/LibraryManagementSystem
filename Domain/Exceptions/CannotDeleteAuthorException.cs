namespace Domain.Exceptions
{
    public class CannotDeleteAuthorException(int authorId) : Exception($"Author {authorId} cannot be deleted, because it has books!");
}
