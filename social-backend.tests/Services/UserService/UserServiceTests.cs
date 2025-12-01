using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using social_backend.tests.Data;
using SocialBackend.Dto;
using SocialBackend.Exceptions;
using SocialBackend.Models;
using SocialBackend.Services;

namespace social_backend.tests;

public class UserServiceTests : TestBase
{
    private readonly IUserService _userService;
    private readonly Mock<IPasswordHelper> _mockHelper;
    public UserServiceTests()
    {

        _mockHelper = new Mock<IPasswordHelper>();
        _userService = new UserService(Context, _mockHelper.Object);
    }

    protected override void SeedData()
    {
        Context.Users.Add(new User { Id = 1, Username = "Alfred", Email = "Alfred@microsoft.com", PasswordHash = "Hej123", Description = "Just a lil guy" });
        Context.Users.Add(new User { Id = 2, Username = "Manfred", Email = "Manfred@microsoft.com", PasswordHash = "123Lösen", Description = "Just a medium guy" });
        Context.Users.Add(new User { Id = 3, Username = "Fredrik", Email = "Fredrik@microsoft.com", PasswordHash = "123456", Description = "Just a big guy" });

        Context.SaveChanges();
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUserWithCorrectId()
    {
        //Arrange
        var request = new UserIdRequest
        {
            UserId = 1
        };

        //Act
        var result = await _userService.GetUserById(request);

        //Assert
        Assert.Equal(request.UserId, result.Id);
        Assert.IsType<User>(result);
    }

    [Fact]
    public async Task GetUserById_ShouldThrowWhenNoUserIsFound()
    {
        //Arrange
        var request = new UserIdRequest
        {
            UserId = 1337
        };

        //Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUserById(request));
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnAllUsers()
    {
        //Arrange
        int expected = 3;

        //Act
        var result = await _userService.GetAllUsers();
        int actual = result.Count();

        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task DeleteUser_ShouldDeleteUser()
    {
        //Arrange
        int userId = 3;

        //Act
        await _userService.DeleteUser(userId);

        //Assert
        var deletedUser = await Context.Users.FindAsync(userId);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task DeleteUser_ShouldThrowIfUserIsNotFound()
    {
        //Arrange
        int userId = 1337;

        //Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.DeleteUser(userId));
    }

    [Fact]
    public async Task UpdatePassword_ShouldChangeUpdatedValuesOnSelectedUser()
    {
        //Arrange
        int userId = 3;
        var request = new UpdatePasswordRequest
        {
            NewPassword = "123457"
        };
        _mockHelper.Setup(h => h.HashPassword(It.IsAny<string>())).Returns((string passwordHash) => passwordHash);

        //Act
        await _userService.UpdatePassword(request, userId);
        var user = await Context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        //Assert
        Assert.Equal(request.NewPassword, user?.PasswordHash);
    }

    [Fact]
    public async Task UpdatePassword_ShouldThrowWhenUserIsNotFound()
    {
        //Arrange
        int userId = 1337;
        var request = new UpdatePasswordRequest
        {
            NewPassword = "123457"
        };

        //Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.UpdatePassword(request, userId));
    }

    [Fact]
    public async Task FollowUser_UpdatesFollowingListForLoggedinUser()
    {
        //Arrange
        var loggedInUserId = 1;
        var userRequestToFollow = new UserIdRequest
        {
            UserId = 2,
        };

        //Act
        await _userService.FollowUser(loggedInUserId, userRequestToFollow);

        //Assert
        var loggedInUser = Context.Users.Include(u => u.Following).First(u => u.Id == loggedInUserId);
        var userToFollow = Context.Users.First(u => u.Id == userRequestToFollow.UserId);

        Assert.Contains(userToFollow, loggedInUser.Following);
    }

    [Fact]
    public async Task FollowUser_UpdatesFollowerListForFollowedUser()
    {
        //Arrange
        var loggedInUserId = 1;
        var userRequestToFollow = new UserIdRequest
        {
            UserId = 2,
        };

        //Act
        await _userService.FollowUser(loggedInUserId, userRequestToFollow);

        //Assert
        var loggedInUser = Context.Users.First(u => u.Id == loggedInUserId);
        var userToFollow = Context.Users.Include(u => u.Followers).First(u => u.Id == userRequestToFollow.UserId);

        Assert.Contains(loggedInUser, userToFollow.Followers);
    }

    [Fact]
    public async Task ValidateFollowAsync_ShouldReturnBothUsersWhenValidateSucceeds()
    {
        //Arrange
        var loggedInUserId = 1;
        var userToFollowId = 2;

        //Act
        var (loggedInUser, userToFollow) = await _userService.ValidateFollowAsync(loggedInUserId, userToFollowId);

        //Assert
        var loggedInUserFromDb = Context.Users.First(u => u.Id == loggedInUserId);
        var userToFollowFromDb = Context.Users.First(u => u.Id == userToFollowId);
        Assert.IsType<User>(loggedInUser);
        Assert.IsType<User>(userToFollow);
        Assert.Equal(loggedInUserFromDb, loggedInUser);
        Assert.Equal(userToFollowFromDb, userToFollow);
    }
    [Fact]
    public async Task ValidateFollowAsync_ShouldThrowWhenLoggedInUserIdIsWrong()
    {
        //Arrange
        var loggedInUserId = 500;
        var userToFollowId = 2;

        //Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.ValidateFollowAsync(loggedInUserId, userToFollowId));
    }

    [Fact]
    public async Task ValidateFollowAsync_ShouldThrowWhenUserToFollowIdIsWrong()
    {
        //Arrange
        var loggedInUserId = 1;
        var userToFollowId = 500;

        //Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.ValidateFollowAsync(loggedInUserId, userToFollowId));
    }

    [Fact]
    public async Task ValidateFollowAsync_ShouldThrowWhenBothIdsAreTheSame()
    {
        //Arrange
        var loggedInUserId = 1;
        var userToFollowId = 1;

        //Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _userService.ValidateFollowAsync(loggedInUserId, userToFollowId));
    }

    [Fact]
    public async Task ValidateFollowAsync_ShouldThrowWhenUserIsAlreadyFollowed()
    {
        //Arrange
        var loggedInUserId = 1;
        var userToFollowId = 2;

        var loggedInUser = Context.Users.Include(u => u.Following).First(u => u.Id == loggedInUserId);
        var userToFollow = Context.Users.First(u => u.Id == userToFollowId);
        loggedInUser.Following.Add(userToFollow);
        await Context.SaveChangesAsync();

        //Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _userService.ValidateFollowAsync(loggedInUserId, userToFollowId));
    }

    [Fact]
    public async Task UnfollowUser_UpdatesFollowingListForLoggedinUser()
    {
        //Arrange
        var loggedInUserId = 1;
        var userRequestToFollow = new UserIdRequest
        {
            UserId = 2,
        };

        var loggedInUser = Context.Users.Include(u => u.Following).First(u => u.Id == loggedInUserId);
        var userToFollow = Context.Users.Include(u => u.Followers).First(u => u.Id == userRequestToFollow.UserId);
        loggedInUser.Following.Add(userToFollow);
        userToFollow.Followers.Add(loggedInUser);

        //Act
        await _userService.UnfollowUser(loggedInUserId, userRequestToFollow);

        //Assert

        Assert.DoesNotContain(userToFollow, loggedInUser.Following);
    }

    [Fact]
    public async Task UnfollowUser_UpdatesFollowerListForFollowedUser()
    {
        //Arrange
        var loggedInUserId = 1;
        var userRequestToFollow = new UserIdRequest
        {
            UserId = 2,
        };

        var loggedInUser = Context.Users.Include(u => u.Following).First(u => u.Id == loggedInUserId);
        var userToFollow = Context.Users.Include(u => u.Followers).First(u => u.Id == userRequestToFollow.UserId);
        loggedInUser.Following.Add(userToFollow);
        userToFollow.Followers.Add(loggedInUser);

        //Act
        await _userService.UnfollowUser(loggedInUserId, userRequestToFollow);

        //Assert
        Assert.DoesNotContain(loggedInUser, userToFollow.Followers);
    }

    [Fact]
    public async Task ValidateUnfollowAsync_ShouldReturnBothUsersWhenValidateSucceeds()
    {
        //Arrange
        var loggedInUserId = 1;
        var userToFollowId = 2;

        var loggedInUserFromDb = Context.Users.Include(u => u.Following).First(u => u.Id == loggedInUserId);
        var userToFollowFromDb = Context.Users.Include(u => u.Followers).First(u => u.Id == userToFollowId);
        loggedInUserFromDb.Following.Add(userToFollowFromDb);
        userToFollowFromDb.Followers.Add(loggedInUserFromDb);

        //Act
        var (loggedInUser, userToFollow) = await _userService.ValidateUnfollowAsync(loggedInUserId, userToFollowId);

        //Assert
        Assert.IsType<User>(loggedInUser);
        Assert.IsType<User>(userToFollow);
        Assert.Equal(loggedInUserFromDb, loggedInUser);
        Assert.Equal(userToFollowFromDb, userToFollow);
    }
    [Fact]
    public async Task ValidateUnFollowAsync_ShouldThrowWhenLoggedInUserIdIsWrong()
    {
        //Arrange
        var loggedInUserId = 500;
        var userToFollowId = 2;

        //Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.ValidateUnfollowAsync(loggedInUserId, userToFollowId));
    }

    [Fact]
    public async Task ValidateUnFollowAsync_ShouldThrowWhenUserToFollowIdIsWrong()
    {
        //Arrange
        var loggedInUserId = 1;
        var userToFollowId = 500;

        //Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.ValidateUnfollowAsync(loggedInUserId, userToFollowId));
    }

    [Fact]
    public async Task ValidateUnFollowAsync_ShouldThrowWhenBothIdsAreTheSame()
    {
        //Arrange
        var loggedInUserId = 1;
        var userToFollowId = 1;

        //Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _userService.ValidateUnfollowAsync(loggedInUserId, userToFollowId));
    }

    [Fact]
    public async Task ValidateUnFollowAsync_ShouldThrowWhenUserIsNotFollowed()
    {
        //Arrange
        var loggedInUserId = 1;
        var userToFollowId = 2;

        //Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _userService.ValidateUnfollowAsync(loggedInUserId, userToFollowId));
    }
}