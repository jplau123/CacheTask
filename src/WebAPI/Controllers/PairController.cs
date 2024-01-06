using Application.DTOs.Errors;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace WebAPI.Controllers;

/// <summary>
/// Creates, appends, Deletes and Gets cached objects
/// </summary>
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
        Description = "Creates new key-value pair, if key does not exists. If key already exists, then it overwrites old key-value pair with new one.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Successfully created a key-value pair", typeof(CreatePairResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreatePairRequest request)
    {
        var result =  await _pairService.Create(request);
        return CreatedAtAction(nameof(Get), new {key=result.Key}, result);
    }

    [HttpPost("append")]
    [Consumes(MediaTypeNames.Application.Json)]
    [SwaggerOperation(Summary = "Appends key-value pair",
        Description = "Appends provided list of objects to the list of objects in key-value pair. If key does not exists, it creates new key-value pair.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Successfully created a key-value pair", typeof(AppendPairResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Append([FromBody] AppendPairRequest request)
    {
        var result = await _pairService.Append(request);
        return CreatedAtAction(nameof(Get), new { key = result.Key }, result);
    }

    /// <summary>
    /// Deletes key-value pair
    /// </summary>
    /// <remarks>
    /// Deletes key-value pair by provided key
    /// </remarks>
    [Consumes(MediaTypeNames.Text.Plain)]
    [SwaggerResponse(StatusCodes.Status203NonAuthoritative)]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Key not found", typeof(ErrorViewModel))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    [HttpDelete("{key}")]
    public async Task<IActionResult> Delete([SwaggerParameter("The key of key-value pair to delete.", Required = true)] string key)
    {
        await _pairService.Delete(key);
        return NoContent();
    }

    /// <summary>
    /// Gets key-value pair
    /// </summary>
    /// <remarks>
    /// Gets key-value pair by provided key and renews expiration time.
    /// </remarks>
    /// <param name="key">
    /// The key of key-value pair to retrieve.
    /// </param>
    /// <response code="200">Successfully retrieved a key-value pair</response>
    /// <response code="404">Key not found</response>
    /// <response code="500">Internal Server Error</response>
    [ProducesResponseType(typeof(GetPairResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorViewModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("{key}")]
    public async Task<IActionResult>Get(string key)
    {
        return Ok( await _pairService.Get(key));
    }
}
