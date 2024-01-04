namespace Application.DTOs.Requests;

public record CreatePairRequest
{
    public string Key { get; set; } = "";
    public List<object> Value { get; set; } = [];
    public TimeSpan ExpirationPeriod { get; set; } = TimeSpan.FromDays(0);
}
