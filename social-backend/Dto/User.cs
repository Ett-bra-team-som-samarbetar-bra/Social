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
    public required int UserId { get; set; }
    public required string NewPassword { get; set; }
}

public record UserIdRequest
{
    public required int UserId { get; set; }
}