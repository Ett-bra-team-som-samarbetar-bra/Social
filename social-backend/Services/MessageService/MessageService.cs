public class MessageService(IDatabaseContext context) : IMessageService
{
    private readonly IDatabaseContext _context = context;

    // remove later and use UserService
    public async Task<User> FindUserOrThrowAsync(int userId)
    {
        return await _context.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException("User with this ID not found.");
    }

    public async Task<PaginatedList<Message>> GetMessagesBetweenUsersAsync(
        int userAId, int userBId, int pageIndex, int pageSize)
    {
        var sender = await FindUserOrThrowAsync(userAId);
        var receiver = await FindUserOrThrowAsync(userBId);

        var query = _context.Messages
            .Where(m => (m.SendingUserId == userAId && m.ReceivingUserId == userBId) ||
                        (m.SendingUserId == userBId && m.ReceivingUserId == userAId))
            .OrderBy(m => m.CreatedAt);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var messages = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<Message>(messages, pageIndex, totalPages);
    }

    public async Task<Message> SendMessageAsync(int senderId, int receiverId, string content)
    {
        var sender = await FindUserOrThrowAsync(senderId);
        var receiver = await FindUserOrThrowAsync(receiverId);

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