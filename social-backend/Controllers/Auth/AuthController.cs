using Microsoft.AspNetCore.Authorization;

namespace SocialBackend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var user = await _authService.Login(request, HttpContext);
        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        await _authService.RegisterAsync(request);
        return Ok(new { message = "Register successful" });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<User>> Me()
    {
        var loggedInUser = await _authService.GetLoggedInUser(HttpContext);
        return Ok(loggedInUser);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        _authService.Logout(HttpContext);
        return Ok("Logout successful");
    }
}
