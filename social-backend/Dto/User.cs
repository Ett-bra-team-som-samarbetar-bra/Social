namespace SocialBackend.Dto;

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
    public required string Description { get; set; }
}

public record UpdatePasswordRequest
{
    public required string NewPassword { get; set; }
}

public record UserIdRequest
{
    public required int UserId { get; set; }
}

public record UpdateDescriptionRequest
{
    public required string NewDescription { get; set; }
}


public class UserDto
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Description { get; set; }
    public List<int> LikedPostIds { get; set; } = [];
}
