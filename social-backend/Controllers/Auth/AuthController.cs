namespace social_backend.Controllers;

[ApiController]
[Route("api/auth/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        await _authService.Login(request, HttpContext);
        return Ok("Log in successful");
    }

    public async Task<ActionResult> Register(RegisterRequest request)
    {
        await _authService.RegisterAsync(request);
        return Ok("Register successful");
    }

    public async Task<ActionResult> Me()
    {
        await _authService.GetLoggedInUser(HttpContext);
        return Ok();
    }
}
