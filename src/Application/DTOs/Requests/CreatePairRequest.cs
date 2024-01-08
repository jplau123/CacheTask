using System.ComponentModel;
using Swashbuckle.AspNetCore.Annotations;

namespace Application.DTOs.Requests;

public record CreatePairRequest
{
    [SwaggerSchema("Key for the key-value pair")]
    [DefaultValue("key")]
    public string Key { get; set; } = "";

    [SwaggerSchema("List of objects as a value for key-value pair")]
    public List<object> Value { get; set; } = [];

    [SwaggerSchema("Expiration period in seconds")]
    [DefaultValue(600)]
    public int? ExpirationPeriodInSeconds { get; set; }
}
