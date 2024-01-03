namespace Application.DTOs.Errors;

public class ErrorModel
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Trace { get; set; } = string.Empty;
}
