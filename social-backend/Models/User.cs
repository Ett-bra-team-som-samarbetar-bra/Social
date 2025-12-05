namespace SocialBackend.Models;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Description { get; set; } = "";
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public ICollection<User> Following { get; set; } = [];
    public ICollection<User> Followers { get; set; } = [];
    public ICollection<Message> MessagesSent { get; set; } = [];
    public ICollection<Message> MessagesReceived { get; set; } = [];
    public ICollection<Post> Posts { get; set; } = [];
    public ICollection<Post> LikedPosts { get; set; } = [];
}
