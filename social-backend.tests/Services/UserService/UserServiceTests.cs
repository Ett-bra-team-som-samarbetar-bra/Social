using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using social_backend.tests.Data;
using SocialBackend.Dto;
using SocialBackend.Exceptions;
using SocialBackend.Models;
using SocialBackend.Services;

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
        await Assert.ThrowsAsync<UserNotFoundException>(() => _userService.GetUserById(request));
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
        var request = new UserIdRequest
        {
            UserId = 3
        };

        //Act
        await _userService.DeleteUser(request);

        //Assert
        var deletedUser = await Context.Users.FindAsync(request.UserId);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task DeleteUser_ShouldThrowIfUserIsNotFound()
    {
        //Arrange
        var request = new UserIdRequest
        {
            UserId = 1337
        };

        //Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => _userService.DeleteUser(request));
    }

    [Fact]
    public async Task UpdatePassword_ShouldChangeUpdatedValuesOnSelectedUser()
    {
        //Arrange
        var request = new UpdatePasswordRequest
        {
            UserId = 3,
            NewPassword = "123457"
        };
        _mockHelper.Setup(h => h.HashPassword(It.IsAny<string>())).Returns((string passwordHash) => passwordHash);

        //Act
        await _userService.UpdatePassword(request);
        var user = await Context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

        //Assert
        Assert.Equal(request.NewPassword, user?.PasswordHash);
    }

    [Fact]
    public async Task UpdatePassword_ShouldThrowWhenUserIsNotFound()
    {
        //Arrange
        var request = new UpdatePasswordRequest
        {
            UserId = 1337,
            NewPassword = "123457"
        };

        //Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => _userService.UpdatePassword(request));
    }
}