public interface IAuthService
{
    string HashPassword(string password);
    bool PasswordIsVerified(string password, string storedSalt);
}