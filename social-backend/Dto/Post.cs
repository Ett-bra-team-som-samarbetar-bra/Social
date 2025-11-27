namespace SocialBackend.Dto;

public record CreatePostDto
{
    public required User User { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
}

public record EditPostDto
{
    public required User User { get; set; }
    public int Id { get; set; }
    public required string Content { get; set; }
}

public record CreateCommentDto
{
    public required User User { get; set; }
    public required string Content { get; set; }
}
