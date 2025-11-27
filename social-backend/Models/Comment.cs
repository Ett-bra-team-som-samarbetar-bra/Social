namespace SocialBackend.Models;

public class Comment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}
