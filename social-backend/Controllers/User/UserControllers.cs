using social_backend.Migrations;

namespace social_backend.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("GetUserById")]
    public async Task<ActionResult<User>> GetUserById(UserIdRequest request)
    {
        var user = await _userService.GetUserById(request);
        return Ok(user);
    }

    [HttpGet("GetAllUsers")]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsers();
        return Ok(users);
    }

    [HttpPut("update-password")]
    public async Task<ActionResult> UpdatePassword(UpdatePasswordRequest request)
    {
        var userId = HttpContext.GetUserId();
        await _userService.UpdatePassword(request, userId);
        return Ok(new { message = "Password has been updated" });
    }

    [HttpPut("update-description")]
    public async Task<ActionResult> UpdateDescription(UpdateDescriptionRequest request)
    {
        var userId = HttpContext.GetUserId();
        await _userService.UpdateDescription(request, userId);
        return Ok(new { message = "Description has been updated" });
    }

    [HttpDelete("DeleteUser")]
    public async Task<ActionResult> DeleteUser()
    {
        var userId = HttpContext.GetUserId();
        await _userService.DeleteUser(userId);
        return NoContent();
    }

    [HttpPut("FollowUser")]
    public async Task<ActionResult> FollowUser(UserIdRequest request)
    {
        var userId = HttpContext.GetUserId();
        await _userService.FollowUser(userId, request);
        return Ok(new { message = "User followed successfully" });
    }

    [HttpPut("UnfollowUser")]
    public async Task<ActionResult> UnfollowUser(UserIdRequest request)
    {
        var userId = HttpContext.GetUserId();
        await _userService.UnfollowUser(userId, request);
        return Ok(new { message = "User unfollowed successfully" });
    }
}
