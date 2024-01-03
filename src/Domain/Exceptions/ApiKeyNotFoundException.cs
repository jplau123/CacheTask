namespace Domain.Exceptions;

public class ApiKeyNotFoundException : Exception
{
    public ApiKeyNotFoundException(string? message = "") : base(message) { }
}
