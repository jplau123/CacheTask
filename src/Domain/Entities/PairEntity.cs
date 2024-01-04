namespace Domain.Entities;

public class PairEntity
{
    public int Id { get; set; }
    public string Key { get; set; } = "";
    public List<object> Value { get; set; } = [];
    public DateTime ExpiresAt { get; set; }
    public int? ExpirationPeriodInSeconds { get; set; }
}