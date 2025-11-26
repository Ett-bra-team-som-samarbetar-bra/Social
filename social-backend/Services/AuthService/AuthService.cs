namespace SocialBackend.Services;

using BCrypt.Net;

public class AuthService(DatabaseContext dbContext) : IAuthService
{
    public readonly IDatabaseContext _db = dbContext;

    public async Task RegisterAsync(RegisterRequest request)
    {
        if (await DoesUserExists(request.Username))
            throw new Exception("Username already exists");

        var passwordHash = HashPassword(request.Password);
        var user = CreateUser(request, passwordHash);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task<User> Login(LoginRequest request, HttpContext context)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
        if (user == null)
            throw new Exception("Invalid username or password");

        if (IsPasswordVerified(request.Password, user.PasswordHash))
            SetUserSession(user, context);
        else
            throw new Exception("Invalid username or password");

        return user;
    }

    public void Logout(HttpContext context) => context.Session.Clear();
    public void SetUserSession(User user, HttpContext context) => context.Session.SetInt32("UserId", user.Id);

    public string HashPassword(string password)
    {
        return BCrypt.HashPassword(password, workFactor: 12);
    }

    public bool IsPasswordVerified(string password, string storedHash)
    {
        return BCrypt.Verify(password, storedHash);
    }

    public async Task<bool> DoesUserExists(string username)
    {
        return await _db.Users.AnyAsync(u => u.Username == username);
    }

    public User CreateUser(RegisterRequest request, string passwordHash)
    {
        return new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash
        };
    }
}

public record LoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public record RegisterRequest
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}