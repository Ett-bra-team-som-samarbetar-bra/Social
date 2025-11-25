namespace SocialBackend.Services;

public interface IMessageService
{
      Message SendMessage(int senderId, int receiverId, string content);

}
