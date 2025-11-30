namespace SocialBackend.Exceptions
{
    public class UserNotFoundException(int userId) : Exception($"User with ID {userId} was not found");
    public class InvalidCredentialsException() : Exception("Invalid username or password");
    public class UsernameExistsException() : Exception("Username already exists");
    public class CannotFollowSelfException() : Exception("Unable to follow your own account");
    public class CannotUnfollowSelfException() : Exception("Unable to unfollow your own account");
    public class AlreadyFollowingException() : Exception("Unable to follow someone you are already following");
    public class NotFollowingException() : Exception("Unable to unfollow someone you are not currently following");
}
