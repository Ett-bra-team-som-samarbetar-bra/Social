namespace SocialBackend.Helpers;

public interface IPasswordHelper
{
    string HashPassword(string password);

    bool IsPasswordVerified(string password, string storedHash);
}