namespace Application.DTOs.Requests;

public record AppendPairRequest
{
    public string Key { get; set; } = "";
    public List<object> Value { get; set; } = [];
    public TimeSpan ExpirationPeriod { get; set; }
}
