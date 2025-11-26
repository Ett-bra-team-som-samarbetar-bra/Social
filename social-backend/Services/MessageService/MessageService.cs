public class MessageService(IDatabaseContext context) : IMessageService
{
    private readonly IDatabaseContext _context = context;

    public async Task<Message> SendMessageAsync(int senderId, int receiverId, string content)
    {
        var sender = await _context.Users.FindAsync(senderId)
            ?? throw new ArgumentException("Sender not found", nameof(senderId));
        var receiver = await _context.Users.FindAsync(receiverId)
            ?? throw new ArgumentException("Receiver not found", nameof(receiverId));

        var message = new Message
        {
            SendingUserId = senderId,
            ReceivingUserId = receiverId,
            Content = content,
            SendingUser = sender,
            ReceivingUser = receiver
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return message;
    }
}