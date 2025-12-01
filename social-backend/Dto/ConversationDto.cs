public class ConversationDto
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public bool HasUnreadMessages { get; set; }
    public DateTime LastMessageAt { get; set; }
}
