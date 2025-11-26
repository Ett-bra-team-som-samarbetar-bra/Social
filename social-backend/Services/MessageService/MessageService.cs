public class MessageService(IDatabaseContext context) : IMessageService
{
    private readonly IDatabaseContext _context = context;

    // remove later and use UserService and then mock dependency
    public async Task<User> FindUserOrThrowAsync(int userId, string paramName)
    {
        return await _context.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException($"{paramName} with this ID not found.");
    }
    public async Task<Message> SendMessageAsync(int senderId, int receiverId, string content)
    {
        var sender = await FindUserOrThrowAsync(senderId, "Sender");
        var receiver = await FindUserOrThrowAsync(receiverId, "Receiver");

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Message cannot be empty!", nameof(content));
        }

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