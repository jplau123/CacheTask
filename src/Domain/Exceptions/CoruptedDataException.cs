namespace Domain.Exceptions
{
    public class CoruptedDataException : Exception
    {
        public CoruptedDataException(string? message = "") : base(message) { }
    }
}