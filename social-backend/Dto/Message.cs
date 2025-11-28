public record MessageDto(
    int SendingUserId,
    string SendingUserName,
    int ReceivingUserId,
    string ReceivingUserName,
    DateTime CreatedAt,
    string Content
);

public record SendMessageRequest
{
    public int SendingUserId { get; set; }
    public int ReceivingUserId { get; set; }
    public string Content { get; set; }
}
