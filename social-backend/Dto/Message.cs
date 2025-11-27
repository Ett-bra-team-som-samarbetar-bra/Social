public record MessageDto(
    int Id,
    int SendingUserId,
    string SendingUserName,
    int ReceivingUserId,
    string ReceivingUserName,
    DateTime CreatedAt,
    string Content
);