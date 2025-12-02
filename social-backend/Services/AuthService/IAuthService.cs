public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task<User> Login(LoginRequest request, HttpContext context);
    void Logout(HttpContext context);
    void SetUserSession(User user, HttpContext context);
    Task<bool> DoesUserExist(string username);
    User CreateUser(RegisterRequest request, string passwordHash);
    Task<User> GetLoggedInUser(HttpContext context);
}