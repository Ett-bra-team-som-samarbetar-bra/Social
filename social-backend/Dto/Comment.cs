namespace SocialBackend.Dto;

public record CommentCreateDto
{
    public required User User { get; set; }
    public required string Content { get; set; }
}

public record CommentResponseDto
{
    public int UserId { get; set; }
    public required string UserName { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
