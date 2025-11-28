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
    public int ReceivingUserId { get; set; }
    public required string Content { get; set; }
}
