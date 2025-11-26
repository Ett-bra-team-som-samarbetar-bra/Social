namespace SocialBackend.Models;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public ICollection<User> Following { get; set; } = new List<User>();
    public ICollection<User> Followers { get; set; } = new List<User>();
    public ICollection<Message> MessagesSent { get; set; } = new List<Message>();
    public ICollection<Message> MessagesReceived { get; set; } = new List<Message>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Post> LikedPosts = new List<Post>();

}