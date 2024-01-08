using System.ComponentModel;
using Swashbuckle.AspNetCore.Annotations;

namespace Application.DTOs.Responses;

public record CreatePairResponse
{
    [SwaggerSchema("Key for key-value pair")]
    [DefaultValue("key")]
    public string Key { get; set; } = "";

    [SwaggerSchema("List of objects as a value for key-value pair")]
    public List<object> Value { get; set; } = [];

    [SwaggerSchema("Time when key-value pair expires")]
    public DateTime ExpiresAt { get; set; }

    [SwaggerSchema("Expiration period in seconds")]
    [DefaultValue(600)]
    public int? ExpirationPeriodInSeconds { get; set; }
}
