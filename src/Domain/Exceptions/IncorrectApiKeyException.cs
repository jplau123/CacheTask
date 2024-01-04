namespace Domain.Exceptions;

public class IncorrectApiKeyException : Exception
{
    public IncorrectApiKeyException(string? message = "") : base(message) { }
}
