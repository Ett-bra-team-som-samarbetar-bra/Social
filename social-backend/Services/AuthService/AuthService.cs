namespace SocialBackend.Services;

using BCrypt.Net;

public class AuthService(DatabaseContext dbContext, IPasswordHelper passwordHelper) : IAuthService
{
    private readonly IDatabaseContext _db = dbContext;
    private readonly IPasswordHelper _passwordHelper = passwordHelper;

    public async Task RegisterAsync(RegisterRequest request)
    {
        if (await DoesUserExist(request.Username))
            throw new BadRequestException("Username already exists");

        var passwordHash = _passwordHelper.HashPassword(request.Password);
        var user = CreateUser(request, passwordHash);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task<User> Login(LoginRequest request, HttpContext context)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
        if (user == null)
            throw new BadRequestException("Incorrect username or password");

        if (_passwordHelper.IsPasswordVerified(request.Password, user.PasswordHash))
            SetUserSession(user, context);
        else
            throw new BadRequestException("Incorrect username or password");

        return user;
    }

    public void Logout(HttpContext context) => context.Session.Clear();
    public void SetUserSession(User user, HttpContext context) => context.Session.SetInt32("UserId", user.Id);

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

    public async Task<User> GetLoggedInUser(HttpContext context)
    {
        var userId = context.Session.GetInt32("UserId");

        var loggedInUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new NotFoundException("No user logged in");

        return loggedInUser;
    }
}