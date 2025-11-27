namespace SocialBackend.Models;

public class Message
{
    public int Id { get; set; }
    public int SendingUserId { get; set; }
    public  User SendingUser { get; set; } = null!;
    public int ReceivingUserId { get; set; }
    public  User ReceivingUser { get; set; } = null!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public required string Content { get; set; }
}
