
public class MessageService(IDatabaseContext context) : IMessageService
{
    private readonly IDatabaseContext _context = context;

    // remove later and use UserService
    private async Task<User> ValidateUserExists(int userId)
    {
        return await _context.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
    }

    public async Task<PaginatedList<MessageDto>> GetMessagesBetweenUsersAsync(
        int sendingUserId, int receivingUserId, int pageIndex, int pageSize)
    {
        await ValidateUserExists(sendingUserId);
        await ValidateUserExists(receivingUserId);

        if (pageIndex < 1)
            throw new ArgumentOutOfRangeException(nameof(pageIndex), "Page index must be greater than 0.");
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than 0.");

        var query = _context.Messages
            .Where(m => (m.SendingUserId == sendingUserId && m.ReceivingUserId == receivingUserId) ||
                        (m.SendingUserId == receivingUserId && m.ReceivingUserId == sendingUserId))
            .OrderBy(m => m.CreatedAt);

        var count = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(count / (double)pageSize);

        var messages = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MessageDto(
                m.Id,
                m.SendingUserId,
                m.SendingUser.Username,
                m.ReceivingUserId,
                m.ReceivingUser.Username,
                m.CreatedAt,
                m.Content))
            .ToListAsync();

        return new PaginatedList<MessageDto>(messages, pageIndex, totalPages);
    }

    public async Task<MessageDto> SendMessageAsync(int sendingUserId, int receivingUserId, string content)
    {
        if (sendingUserId == receivingUserId)
            throw new InvalidOperationException("Cannot send a message to oneself.");

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message cannot be empty!", nameof(content));

        var sendingUser = await ValidateUserExists(sendingUserId);
        var receivingUser = await ValidateUserExists(receivingUserId);

        var message = new Message
        {
            SendingUserId = sendingUserId,
            ReceivingUserId = receivingUserId,
            SendingUser = sendingUser,
            ReceivingUser = receivingUser,
            Content = content
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return new MessageDto(
            message.Id,
            message.SendingUserId,
            message.SendingUser.Username,
            message.ReceivingUserId,
            message.ReceivingUser.Username,
            message.CreatedAt,
            message.Content
        );
    }
}

public record MessageDto(
    int Id,
    int SendingUserId,
    string SendingUserName,
    int ReceivingUserId,
    string ReceivingUserName,
    DateTime CreatedAt,
    string Content
);
