using System.ComponentModel.DataAnnotations.Schema;

namespace SocialBackend.Models;

public class Post
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public required string Title { get; set; }
    public required string Content { get; set; }
    public int LikeCount { get; set; } = 0;
    public ICollection<Comment> Comments { get; set; } = [];
    public bool IsEdited => CreatedAt != UpdatedAt;
}
