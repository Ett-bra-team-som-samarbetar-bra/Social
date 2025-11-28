namespace SocialBackend.Controllers;

[Route("api/[controller]")]
public class PutController : ControllerBase
{
    [HttpPut]
    public IActionResult Put()
    {
        return Ok("Put!");
    }
}

