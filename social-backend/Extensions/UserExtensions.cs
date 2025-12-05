using SocialBackend.Models;
using SocialBackend.Dto; // adjust namespace based on your structure
using System.Linq;

public static class UserExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Description = user.Description,
            LikedPostIds = user.LikedPosts.Select(p => p.Id).ToList()
        };
    }
}