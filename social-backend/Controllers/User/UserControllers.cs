using Microsoft.AspNetCore.Authorization;

namespace SocialBackend.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [Authorize]
    [HttpGet("id/{profileId}")]
    public async Task<ActionResult<User>> GetUserById(int profileId)
    {
        var userId = HttpContext.GetUserId();
        var profile = await _userService.GetUserProfile(profileId, userId);
        return Ok(profile);
    }

    [Authorize]
    [HttpGet("GetAllUsers")]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsers();
        return Ok(users);
    }

    [Authorize]
    [HttpPut("update-password")]
    public async Task<ActionResult> UpdatePassword(UpdatePasswordRequest request)
    {
        var userId = HttpContext.GetUserId();
        await _userService.UpdatePassword(request, userId);
        return Ok(new { message = "Password has been updated" });
    }

    [Authorize]
    [HttpPut("update-description")]
    public async Task<ActionResult> UpdateDescription(UpdateDescriptionRequest request)
    {
        var userId = HttpContext.GetUserId();
        await _userService.UpdateDescription(request, userId);
        return Ok(new { message = "Description has been updated" });
    }

    [Authorize]
    [HttpDelete("DeleteUser")]
    public async Task<ActionResult> DeleteUser()
    {
        var userId = HttpContext.GetUserId();
        await _userService.DeleteUser(userId);
        return NoContent();
    }

    [Authorize]
    [HttpPut("follow")]
    public async Task<ActionResult> FollowUser(UserIdRequest request)
    {
        var userId = HttpContext.GetUserId();
        await _userService.FollowUser(userId, request);
        return Ok(new { isFollowing = true });
    }

    [Authorize]
    [HttpPut("unfollow")]
    public async Task<ActionResult> UnfollowUser(UserIdRequest request)
    {
        var userId = HttpContext.GetUserId();
        await _userService.UnfollowUser(userId, request);
        return Ok(new { isFollowing = false });
    }
}
