using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PairController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok();
    }
}
