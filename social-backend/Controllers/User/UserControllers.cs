namespace social_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    public async Task<ActionResult<User>> GetUserById(UserIdRequest request)
    {
        var user = await _userService.GetUserById(request);
        return Ok(user);
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsers();
        return Ok(users);
    }

    [HttpPut]
    public async Task<ActionResult> UpdatePassword(UpdatePasswordRequest request)
    {
        var userId = HttpContext.GetUserId();
        await _userService.UpdatePassword(request, userId);
        return Ok("Password has been updated");
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteUser()
    {
        var userId = HttpContext.GetUserId();
        await _userService.DeleteUser(userId);
        return NoContent();
    }





}
