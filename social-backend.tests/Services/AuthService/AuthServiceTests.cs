using SocialBackend.tests.Data;
using SocialBackend.Models;
using SocialBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Linq;
using Moq;
using SocialBackend.Dto;
using SocialBackend.Exceptions;
using SocialBackend.Helpers;

namespace SocialBackend.tests.Services;

public class AuthServiceTest : TestBase
{
    private readonly IAuthService _authService;
    private readonly DefaultHttpContext _httpContext;
    private readonly TestSession _session;
    private readonly Mock<IPasswordHelper> _mockHelper;
    private readonly Mock<IAuthenticationService> _authServiceMock;
    public AuthServiceTest()
    {
        _mockHelper = new Mock<IPasswordHelper>();
        _authServiceMock = new Mock<IAuthenticationService>();
        _authService = new AuthService(Context, _mockHelper.Object, CreateLogger<AuthService>());

        _httpContext = new DefaultHttpContext();
        _session = new TestSession();
        _httpContext.Features.Set<ISessionFeature>(new SessionFeature { Session = _session });

        _authServiceMock
            .Setup(a => a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties?>()))
            .Returns(Task.CompletedTask);
        _authServiceMock
            .Setup(a => a.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties?>()))
            .Returns(Task.CompletedTask);

        _httpContext.RequestServices = new ServiceCollection()
            .AddSingleton(_authServiceMock.Object)
            .BuildServiceProvider();
    }

    protected override void SeedData()
    {
        Context.Users.Add(new User { Id = 1, Username = "Alfred", Email = "Alfred@microsoft.com", PasswordHash = "Hej123", Description = "Just a lil guy" });
        Context.Users.Add(new User { Id = 2, Username = "Manfred", Email = "Manfred@microsoft.com", PasswordHash = "123Lösen", Description = "Just a medium guy" });
        Context.Users.Add(new User { Id = 3, Username = "Fredrik", Email = "Fredrik@microsoft.com", PasswordHash = "123456", Description = "Just a big guy" });

        Context.SaveChanges();
    }

    [Fact]
    public void CreateUser_ShouldCreateANewUser()
    {
        //Arrange
        string userName = "Nisse";
        string email = "Nisse@manpower.se";
        string password = "Password";
        string description = "Legendary";
        RegisterRequest newUser = new RegisterRequest
        {
            Username = userName,
            Email = email,
            Password = password,
            Description = description
        };
        var passwordHash = newUser.Password;

        //Act
        var result = _authService.CreateUser(newUser, passwordHash);

        //Assert
        Assert.Equal(userName, result.Username);
        Assert.IsType<User>(result);
    }

    [Fact]
    public async Task DoesUserExist_ShouldReturnTrueWhenUserExists()
    {
        //Arrange
        var username = "Alfred";

        //Act
        var result = await _authService.DoesUserExist(username);

        //Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DoesUserExist_ShouldReturnFalseWhenUserDoesntExist()
    {
        //Arrange
        var username = "Niklas";

        //Act
        var result = await _authService.DoesUserExist(username);

        //Assert
        Assert.False(result);
    }

    [Fact]
    public void SetUserSession_ShouldStoreUserId()
    {
        //Arrange
        var user = new User { Id = 4, Username = "Emil", Email = "Emil@microsoft.com", PasswordHash = "Dansbandsveckanimalung2022", Description = "Legendary" };
        var expected = 4;


        //Act
        _authService.SetUserSession(user, _httpContext);

        //Assert
        var actual = _httpContext.Session.GetInt32("UserId");
        Assert.Equal(expected, actual);
        _authServiceMock.Verify(a => a.SignInAsync(
            _httpContext,
            "AuthCookie",
            It.Is<ClaimsPrincipal>(p =>
                p.Claims.Any(c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString()) &&
                p.Identity != null &&
                p.Identity.Name == user.Username),
            It.IsAny<AuthenticationProperties?>()),
            Times.Once);
    }

    [Fact]
    public void Logout_ShouldRemoveStoredUserId()
    {
        //Arrange
        _httpContext.Session.SetInt32("UserId", 1337);


        //Act
        _authService.Logout(_httpContext);

        //Assert
        Assert.False(_httpContext.Session.TryGetValue("UserId", out _));
        Assert.Empty(_session.Keys);
        _authServiceMock.Verify(a => a.SignOutAsync(_httpContext, "AuthCookie", It.IsAny<AuthenticationProperties?>()), Times.Once);
    }

    [Fact]
    public async Task Login_ShouldReturnUserWHenSuccesfullyLogsIn()
    {
        //Arrange
        var request = new LoginRequest
        {
            Username = "Alfred",
            Password = "Hej123"
        };
        _mockHelper
        .Setup(h => h.IsPasswordVerified(
            It.IsAny<string>(),
            It.IsAny<string>()))
        .Returns((string password, string hash) => password == hash);

        //Act
        var result = await _authService.Login(request, _httpContext);

        //Assert
        Assert.Equal(request.Username, result.Username);
        _authServiceMock.Verify(a => a.SignInAsync(
            _httpContext,
            "AuthCookie",
            It.Is<ClaimsPrincipal>(p =>
                p.Claims.Any(c => c.Type == ClaimTypes.NameIdentifier && c.Value == "1") &&
                p.Identity != null &&
                p.Identity.Name == request.Username),
            It.IsAny<AuthenticationProperties?>()),
            Times.Once);
    }

    [Fact]
    public async Task Login_ShouldThrowWhenUserDoesNotExist()
    {
        var request = new LoginRequest
        {
            Username = "Plåtniklas",
            Password = "Grodan Boll"
        };

        await Assert.ThrowsAsync<BadRequestException>(() => _authService.Login(request, _httpContext));
    }

    [Fact]
    public async Task Login_ShouldThrowWhenPasswordIsIncorrect()
    {
        var request = new LoginRequest
        {
            Username = "Alfred",
            Password = "Tjuvar"
        };
        _mockHelper
        .Setup(h => h.IsPasswordVerified(
            It.IsAny<string>(),
            It.IsAny<string>()))
        .Returns((string password, string hash) => password == hash);

        await Assert.ThrowsAsync<BadRequestException>(() => _authService.Login(request, _httpContext));
    }

    [Fact]
    public async Task RegisterAsync_ShouldAddUserToTheDataBaseIfUserDoesNotExist()
    {
        //Arrange
        var newUser = new RegisterRequest
        {
            Username = "Pelle",
            Email = "Pelle@Microsoft.com",
            Password = "Hemligtsomfän",
            Description = "Cool Kille"
        };
        _mockHelper.Setup(h => h.HashPassword(It.IsAny<string>())).Returns((string passwordHash) => passwordHash);
        var expectedUserCount = Context.Users.Count() + 1;

        //Act
        await _authService.RegisterAsync(newUser);
        var actualUserCount = Context.Users.Count();

        //Assert
        Assert.Equal(expectedUserCount, actualUserCount);
    }
    [Fact]
    public async Task RegisterAsync_ShouldThrowWhenUserExists()
    {
        //Arrange
        var newUser = new RegisterRequest
        {
            Username = "Alfred",
            Email = "Alfred@Microsoft.com",
            Password = "Hemligtsomfän",
            Description = "Cool Kille"
        };
        _mockHelper.Setup(h => h.HashPassword(It.IsAny<string>())).Returns((string passwordHash) => passwordHash);

        //Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _authService.RegisterAsync(newUser));
    }

    [Fact]
    public async Task GetLoggedInUser_ShouldReturnUser_WhenSessionHasValidUserId()
    {
        // Arrange
        _session.SetInt32("UserId", 1); // session contains userId = 1

        // Act
        var user = await _authService.GetLoggedInUser(_httpContext);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(1, user.Id);
        Assert.Equal("Alfred", user.Username);
    }

    [Fact]
    public async Task GetLoggedInUser_ShouldThrowNotFound_WhenSessionHasNoUserId()
    {
        // Arrange
        // No value set in session

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _authService.GetLoggedInUser(_httpContext)
        );
    }

    [Fact]
    public async Task GetLoggedInUser_ShouldThrowNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _session.SetInt32("UserId", 999); // session references a missing user

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _authService.GetLoggedInUser(_httpContext)
        );
    }
}
