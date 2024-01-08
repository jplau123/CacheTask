using Application.DTOs.Errors;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace WebAPI.Controllers;

[SwaggerTag("Creates, appends, Deletes and Gets cached objects")]
[Route("pairs")]
[ApiController]
public class PairController : ControllerBase
{
    private readonly IPairService _pairService;

    public PairController(IPairService pairService)
    {
        _pairService = pairService;
    }

    [HttpPost("create")]
    [Consumes(MediaTypeNames.Application.Json)]
    [SwaggerOperation(Summary = "Creates key-value pair", 
        Description = "Creates new key-value pair, if key does not exists. If key already exists, then it overwrites old key-value pair with new one")]
    [SwaggerResponse(StatusCodes.Status201Created, "Successfully created a key-value pair", typeof(CreatePairResponse), MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreatePairRequest request)
    {
        var result =  await _pairService.Create(request);
        return CreatedAtAction(nameof(Get), new {key=result.Key}, result);
    }

    [HttpPost("append")]
    [Consumes(MediaTypeNames.Application.Json)]
    [SwaggerOperation(Summary = "Appends key-value pair",
        Description = "Appends provided list of objects to the list of objects in key-value pair. If key does not exists, it creates new key-value pair")]
    [SwaggerResponse(StatusCodes.Status201Created, "Successfully created a key-value pair", typeof(AppendPairResponse), MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Append([FromBody] AppendPairRequest request)
    {
        var result = await _pairService.Append(request);
        return CreatedAtAction(nameof(Get), new { key = result.Key }, result);
    }

    [HttpDelete("{key}")]
    [Consumes(MediaTypeNames.Text.Plain)]
    [SwaggerOperation(Summary = "Deletes key-value pair",
        Description = "Deletes key-value pair by provided key")]
    [SwaggerResponse(StatusCodes.Status203NonAuthoritative)]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Key not found", typeof(ErrorViewModel), MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([SwaggerParameter("The key of key-value pair to delete.", Required = true)] string key)
    {
        await _pairService.Delete(key);
        return NoContent();
    }

    [HttpGet("{key}")]
    [Consumes(MediaTypeNames.Text.Plain)]
    [SwaggerOperation(Summary = "Gets key-value pair",
        Description = "Gets key-value pair by provided key and renews expiration time")]
    [SwaggerResponse(StatusCodes.Status200OK, "Found key-value pair", typeof(GetPairResponse), MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Key not found", typeof(ErrorViewModel), MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult>Get([SwaggerParameter("The key of key-value pair to delete.", Required = true)] string key)
    {
        return Ok( await _pairService.Get(key));
    }
}
