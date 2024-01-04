using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("pairs")]
[ApiController]
public class PairController : ControllerBase
{
    private readonly PairService _pairService;
    public PairController(PairService pairService)
    {
        _pairService = pairService;
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody]CreatePairRequest request)
    {
        return Ok(await _pairService.Create(request));
    }

    [HttpPost]
    public async Task<IActionResult> Append([FromBody] AppendPairRequest request)
    {
        return Ok(await _pairService.Append(request));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string key)
    {
        await _pairService.Delete(key);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult>Get(string key)
    {
        return Ok( await _pairService.Get(key));
    }
    

 
}
