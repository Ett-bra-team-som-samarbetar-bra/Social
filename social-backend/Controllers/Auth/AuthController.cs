namespace social_backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var user = await _authService.Login(request, HttpContext);
        return Ok(new { user });
    }
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        await _authService.RegisterAsync(request);
        return Ok(new { message = "Register successful" });
    }
    [HttpGet("me")]
    public async Task<ActionResult<User>> Me()
    {
        var loggedInUser = await _authService.GetLoggedInUser(HttpContext);
        return Ok(loggedInUser);
    }
}
