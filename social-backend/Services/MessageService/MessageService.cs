
namespace SocialBackend.Services;

public interface IMessageService
{
    Task<MessageDto> SendMessageAsync(int senderId, int receiverId, string content);
    Task<List<MessageDto>> GetMessagesBetweenUsersAsync(int userAId, int userBId, int pageSize = 20, DateTime? before = null);
}

public class MessageService(IDatabaseContext context, IUserService userService, IHubContext<ChatHub> hubContext) : IMessageService
{
    private readonly IDatabaseContext _context = context;
    private readonly IUserService _userService = userService;
    private readonly IHubContext<ChatHub> _hubContext = hubContext;

    public async Task<List<MessageDto>> GetMessagesBetweenUsersAsync(
        int sendingUserId,
        int receivingUserId,
        int pageSize = 20,
        DateTime? before = null)
    {
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than 0.");

        var (sendingUser, receivingUser) = await GetBothUsersAsync(sendingUserId, receivingUserId);

        var query = _context.Messages
            .Include(m => m.SendingUser)
            .Include(m => m.ReceivingUser)
            .Where(m => (m.SendingUserId == sendingUserId && m.ReceivingUserId == receivingUserId) ||
                        (m.SendingUserId == receivingUserId && m.ReceivingUserId == sendingUserId));

        if (before.HasValue)
            query = query.Where(m => m.CreatedAt < before.Value);

        var messages = await query
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id)
            .Take(pageSize)
            .Select(m => ToDto(m))
            .ToListAsync();

        messages.Reverse();
        return messages;
    }

    public async Task<MessageDto> SendMessageAsync(int sendingUserId, int receivingUserId, string content)
    {
        if (sendingUserId == receivingUserId)
            throw new InvalidOperationException("Cannot send a message to oneself.");

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message cannot be empty!", nameof(content));

        var (sendingUser, receivingUser) = await GetBothUsersAsync(sendingUserId, receivingUserId);

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
        var messageDto = ToDto(message);
        await BroadcastMessageAsync(messageDto, sendingUserId, receivingUserId);

        return messageDto;
    }

    private static MessageDto ToDto(Message message)
    {
        return new(
            message.SendingUserId,
            message.SendingUser.Username,
            message.ReceivingUserId,
            message.ReceivingUser.Username,
            message.CreatedAt,
            message.Content
        );
    }
    private async Task BroadcastMessageAsync(MessageDto messageDto, int sendingUserId, int receivingUserId)
    {
        await _hubContext.Clients.User(sendingUserId.ToString())
            .SendAsync("ReceiveMessage", messageDto);

        await _hubContext.Clients.User(receivingUserId.ToString())
            .SendAsync("ReceiveMessage", messageDto);
    }

    private async Task<(User SendingUser, User ReceivingUser)> GetBothUsersAsync(int sendingUserId, int receivingUserId)
    {
        var sendingUser = await _userService.GetUserById(new UserIdRequest { UserId = sendingUserId });
        var receivingUser = await _userService.GetUserById(new UserIdRequest { UserId = receivingUserId });
        return (sendingUser, receivingUser);
    }
}