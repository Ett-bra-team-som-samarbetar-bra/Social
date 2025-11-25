namespace SocialBackend.Models;

public class Post
{
    public int Id { get; set; }
    public required User User { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public required string Content { get; set; }
    public required string Title { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<User> Likes { get; set; } = new List<User>();
}