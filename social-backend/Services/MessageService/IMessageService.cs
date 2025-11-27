namespace SocialBackend.Services;

public interface IMessageService
{
      Task<MessageDto> SendMessageAsync(int senderId, int receiverId, string content);
      Task<PaginatedList<MessageDto>> GetMessagesBetweenUsersAsync(int userAId, int userBId, int pageIndex, int pageSize);
}
