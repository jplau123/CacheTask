namespace Domain.Exceptions;

public class ConfigException : Exception
{
    public ConfigException(string? message = "") : base(message) { }
}
