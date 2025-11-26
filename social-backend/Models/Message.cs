namespace SocialBackend.Models;

public class Message
{
    public int Id { get; set; }
    public int SendingUserId { get; set; }
    public required User SendingUser { get; set; }
    public int ReceivingUserId { get; set; }
    public required User ReceivingUser { get; set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public required string Content { get; set; }
}
