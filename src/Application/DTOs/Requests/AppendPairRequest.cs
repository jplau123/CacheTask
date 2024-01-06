using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

namespace Application.DTOs.Requests;

public record AppendPairRequest
{
    [SwaggerSchema("Key for the key-value pair")]
    [DefaultValue("bob")]
    public string Key { get; set; } = "";
    public List<object> Value { get; set; } = [];
}
