using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace SocialBackend.Services;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task<UserDto> Login(LoginRequest request, HttpContext context);
    Task Logout(HttpContext context);
    Task SetUserSession(User user, HttpContext context);
    Task<bool> DoesUserExist(string username);
    User CreateUser(RegisterRequest request, string passwordHash);
    Task<UserDto> GetLoggedInUser(HttpContext context);
}

public class AuthService(DatabaseContext dbContext, IPasswordHelper passwordHelper, ILogger<AuthService> logger) : IAuthService
{
    private readonly IDatabaseContext _db = dbContext;
    private readonly IPasswordHelper _passwordHelper = passwordHelper;
    private readonly ILogger<AuthService> _logger = logger;

    public async Task RegisterAsync(RegisterRequest request)
    {
        if (await DoesUserExist(request.Username))
        {
            _logger.LogWarning("Registration failed: username {Username} already exists", request.Username);
            throw new BadRequestException("Username already exists");
        }

        var passwordHash = _passwordHelper.HashPassword(request.Password);
        var user = CreateUser(request, passwordHash);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        _logger.LogInformation("User {Username} registered with id {UserId}", request.Username, user.Id);
    }

    public async Task<UserDto> Login(LoginRequest request, HttpContext context)
    {
        var user = await _db.Users.Include(u => u.LikedPosts).SingleOrDefaultAsync(u => u.Username == request.Username);
        if (user == null)
        {
            _logger.LogWarning("Login failed: username {Username} not found", request.Username);
            throw new BadRequestException("Incorrect username or password");
        }

        if (_passwordHelper.IsPasswordVerified(request.Password, user.PasswordHash))
            await SetUserSession(user, context);
        else
        {
            _logger.LogWarning("Login failed: invalid password for {Username}", request.Username);
            throw new BadRequestException("Incorrect username or password");
        }

        _logger.LogInformation("User {Username} logged in with id {UserId}", request.Username, user.Id);

        return user.ToDto();
    }

    public async Task Logout(HttpContext context)
    {
        var userId = context.Session.GetInt32("UserId");
        await context.SignOutAsync("AuthCookie");
        context.Session.Clear();
        _logger.LogInformation("User {UserId} logged out", userId);
    }
    public async Task SetUserSession(User user, HttpContext context)
    {
        context.Session.SetInt32("UserId", user.Id);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
        };

        var identity = new ClaimsIdentity(claims, "AuthCookie");
        var principal = new ClaimsPrincipal(identity);

        await context.SignInAsync("AuthCookie", principal);
    }

    public async Task<bool> DoesUserExist(string username)
    {
        return await _db.Users.AnyAsync(u => u.Username == username);
    }

    public User CreateUser(RegisterRequest request, string passwordHash)
    {
        return new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            Description = request.Description
        };
    }

    public async Task<UserDto> GetLoggedInUser(HttpContext context)
    {
        var userId = context.Session.GetInt32("UserId");

        var loggedInUser = await _db.Users.Include(u => u.LikedPosts).FirstOrDefaultAsync(u => u.Id == userId) ?? throw new NotFoundException("No user logged in");
        _logger.LogDebug("Retrieved logged in user {UserId}", loggedInUser.Id);

        return loggedInUser.ToDto();
    }
}
