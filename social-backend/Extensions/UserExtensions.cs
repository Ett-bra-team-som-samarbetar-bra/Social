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

    public static UserProfileDto ToProfileDto(this User user, int? viewerId = null)
    {
        return new UserProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            Description = user.Description,
            CreatedAt = user.CreatedAt,
            PostCount = user.Posts.Count,
            FollowerCount = user.Followers.Count,
            FollowingCount = user.Following.Count,
            IsFollowing = viewerId != null && user.Followers.Any(f => f.Id == viewerId),
            IsOwnProfile = viewerId == user.Id
        };
    }
}