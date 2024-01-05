using Application.DTOs.Requests;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

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
    public async Task<IActionResult> Create([FromBody] CreatePairRequest request)
    {
        var result =  await _pairService.Create(request);
        return CreatedAtAction(nameof(Get), new {key=result.Key}, result);
    }

    [HttpPost("append")]
    public async Task<IActionResult> Append([FromBody] AppendPairRequest request)
    {
        var result = await _pairService.Append(request);
        return CreatedAtAction(nameof(Get), new { key = result.Key }, result);
    }

    [HttpDelete("{key}")]
    public async Task<IActionResult> Delete(string key)
    {
        await _pairService.Delete(key);
        return NoContent();
    }

    [HttpGet("{key}")]
    public async Task<IActionResult>Get(string key)
    {
        return Ok( await _pairService.Get(key));
    }
    

 
}
