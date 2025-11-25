public class Message
{
    public int Id { get; set; }
    public int SendingUserId { get; set; }
    public required User SendingUser { get; set; }
    public int ReceivingUserId { get; set; }
    public required User ReceivingUser { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Content { get; set; }
    public string? Title { get; set; }

}