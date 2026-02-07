namespace Domain.Exceptions
{
    public class DomainRuleViolationException(string message, Exception innerException) : Exception(message, innerException);
}
