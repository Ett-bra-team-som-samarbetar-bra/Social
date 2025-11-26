public interface IAuthService
{
    string HashPassword(string password);
    bool IsPasswordVerified(string password, string storedSalt);
}