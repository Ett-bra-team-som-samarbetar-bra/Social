public class MessageService(IDatabaseContext context) : IMessageService
{
    private readonly IDatabaseContext _context = context;

    // remove later and use UserService
    public async Task<User> FindUserOrThrowAsync(int userId, string paramName)
    {
        return await _context.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException($"{paramName} with this ID not found.");
    }

    public async Task<List<Message>> GetMessagesBetweenUsersAsync(int userAId, int userBId)
    {
        var sender = await FindUserOrThrowAsync(userAId, "User A");
        var receiver = await FindUserOrThrowAsync(userBId, "User B");
        return await _context.Messages
            .Where(m => (m.SendingUserId == userAId && m.ReceivingUserId == userBId) ||
                        (m.SendingUserId == userBId && m.ReceivingUserId == userAId))
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

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