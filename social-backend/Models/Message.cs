namespace SocialBackend.Models;

public class Message
{
    public Message()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public int Id { get; set; }
    public int SendingUserId { get; set; }
    public required User SendingUser { get; set; }
    public int ReceivingUserId { get; set; }
    public required User ReceivingUser { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Content { get; set; }
}
