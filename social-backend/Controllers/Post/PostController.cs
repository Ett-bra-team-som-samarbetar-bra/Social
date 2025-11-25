namespace SocialBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        throw new Exception();
        return Ok("Test successful!");
    }






}



/* namespace Api.Controllers.Auth;

[Route("api/auth")]
[ApiController]
public class LoginController(IAuthService authService, Validator validator) : Controller
{
    private readonly IAuthService _authService = authService;
    private readonly Validator _validator = validator;


    [HttpPost("login")]
    public async Task<ActionResult> LogIn([FromBody] LoginRequest request)
    {
        var validation = _validator.Validate(new LoginValidator(), request);
        if (validation != null)
            return validation;

        var result = await _authService.Login(request.Username, request.Password); //verifying login with authservice.
        if (result == null)
            return NotFound("Invalid username or password");

        var response = new Response(result.Username);

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<ActionResult> Handle([FromBody] RegisterRequest request)
    {
        var validation = _validator.Validate(new RegisterValidator(), request);
        if (validation != null)
            return validation;

        var result = await _authService.Register(request.Username, request.Password, request.Email, request.PhoneNumber); //creating user with authservice
        if (result == null)
        {
            return BadRequest("Username already exists");
        }
        var response = new Response(result.Username);

        return Created("User created", response);
    }
}
 */