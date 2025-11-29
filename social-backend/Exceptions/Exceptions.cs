namespace SocialBackend.Exceptions
{
    public class UserNotFoundException(int userId) : Exception($"User with ID {userId} was not found");
    public class InvalidCredentialsException() : Exception("Invalid username or password");
    public class UsernameExistsException() : Exception("Username already exists");
}
