namespace Application.DTOs.Responses;

public record AppendPairResponse
{
    public string Key { get; set; } = "";
    public List<object> Value { get; set; } = [];
    public DateTime ExpiresAt { get; set; }
    public TimeSpan ExpirationPeriod { get; set; }
}
