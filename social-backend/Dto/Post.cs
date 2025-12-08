namespace SocialBackend.Dto;

public record PostResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Username { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public int LikeCount { get; set; } = 0;
    public ICollection<CommentResponseDto> Comments { get; set; } = [];
    public bool IsEdited => CreatedAt != UpdatedAt;
}

public record PostCreateDto
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}

public record PostEditDto
{
    public int Id { get; set; }
    public required string Content { get; set; }
}

public record PostWithComments
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public List<CommentResponseDto> Comments { get; set; } = [];
}
