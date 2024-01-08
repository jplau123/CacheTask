using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Application.DTOs.Requests;

public record AppendPairRequest
{
    [SwaggerSchema("Key for the key-value pair")]
    [DefaultValue("key")]
    public string Key { get; set; } = "";

    [SwaggerSchema("List of objects as a value for key-value pair")]
    public List<object> Value { get; set; } = [];
}
