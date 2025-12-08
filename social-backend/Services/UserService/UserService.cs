using Microsoft.AspNetCore.Http.HttpResults;

namespace SocialBackend.Services;

public interface IUserService
{
    Task DeleteUser(int userId);
    Task<List<User>> GetAllUsers();
    Task<User> GetUserById(UserIdRequest request);
    Task<UserProfileDto> GetUserProfile(int profileId, int userId);
    Task UpdatePassword(UpdatePasswordRequest request, int userId);
    Task FollowUser(int userId, UserIdRequest request);
    Task<(User, User)> ValidateFollowAsync(int sourceId, int targetId);
    Task UnfollowUser(int userId, UserIdRequest request);
    Task<(User, User)> ValidateUnfollowAsync(int sourceId, int targetId);
    Task UpdateDescription(UpdateDescriptionRequest request, int userId);
}

public class UserService(DatabaseContext dbContext, IPasswordHelper passwordHelper) : IUserService
{
    private readonly IDatabaseContext _db = dbContext;
    private readonly IPasswordHelper _passwordHelper = passwordHelper;

    public async Task<User> GetUserById(UserIdRequest request)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId)
            ?? throw new NotFoundException($"Could not find user with id {request.UserId}");
    }

    public async Task<UserProfileDto> GetUserProfile(int profileId, int userId)
    {
        var user = await _db.Users.Include(u => u.Followers).Include(u => u.Following).Include(u => u.Posts).FirstOrDefaultAsync(u => u.Id == profileId) ?? throw new NotFoundException($"Could not find user with id {profileId}"); ;
        return user.ToProfileDto(userId);
    }

    public async Task<List<User>> GetAllUsers()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task DeleteUser(int userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException($"Could not find user with id {userId}");
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
    }

    public async Task UpdatePassword(UpdatePasswordRequest request, int userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException($"Could not find user with id {userId}");
        user.PasswordHash = _passwordHelper.HashPassword(request.NewPassword);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateDescription(UpdateDescriptionRequest request, int userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException($"Could not find user with id {userId}");
        user.Description = request.NewDescription;
        await _db.SaveChangesAsync();
    }

    public async Task FollowUser(int userId, UserIdRequest request)
    {
        var (loggedInUser, followedUser) = await ValidateFollowAsync(userId, request.UserId);

        loggedInUser.Following.Add(followedUser);
        followedUser.Followers.Add(loggedInUser);

        await _db.SaveChangesAsync();
    }

    public async Task<(User, User)> ValidateFollowAsync(int sourceId, int targetId)
    {
        var loggedInUser = await _db.Users.Include(u => u.Following).FirstOrDefaultAsync(u => u.Id == sourceId)
            ?? throw new NotFoundException($"Could not find user with id {sourceId}");

        var followedUser = await _db.Users.Include(u => u.Followers).FirstOrDefaultAsync(u => u.Id == targetId)
            ?? throw new NotFoundException($"Could not find user with id {sourceId}");

        if (loggedInUser.Id == followedUser.Id)
            throw new BadRequestException("Unable to follow your own account");

        if (loggedInUser.Following.Any(u => u.Id == followedUser.Id))
            throw new BadRequestException("Unable to follow an account you are already following");

        return (loggedInUser, followedUser);
    }

    public async Task UnfollowUser(int userId, UserIdRequest request)
    {
        var (loggedInUser, followedUser) = await ValidateUnfollowAsync(userId, request.UserId);

        loggedInUser.Following.Remove(followedUser);
        followedUser.Followers.Remove(loggedInUser);

        await _db.SaveChangesAsync();
    }

    public async Task<(User, User)> ValidateUnfollowAsync(int sourceId, int targetId)
    {
        var loggedInUser = await _db.Users.Include(u => u.Following).FirstOrDefaultAsync(u => u.Id == sourceId)
            ?? throw new NotFoundException($"Could not find user with id {sourceId}");

        var followedUser = await _db.Users.Include(u => u.Followers).FirstOrDefaultAsync(u => u.Id == targetId)
            ?? throw new NotFoundException($"Could not find user with id {sourceId}");

        if (loggedInUser.Id == followedUser.Id)
            throw new BadRequestException("Unable to unfollow your own account");

        if (!loggedInUser.Following.Any(u => u.Id == followedUser.Id))
            throw new BadRequestException("Unable to unfollow an account you are not following");

        return (loggedInUser, followedUser);
    }
}
