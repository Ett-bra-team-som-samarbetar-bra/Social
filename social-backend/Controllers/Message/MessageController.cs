
namespace SocialBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController(IMessageService messageService) : ControllerBase
{
    private readonly IMessageService _messageService = messageService;

    [HttpPost]
    public async Task<ActionResult<MessageDto>> SendMessage([FromBody] SendMessageRequest request)
    {
        var message = await _messageService.SendMessageAsync(
            request.SendingUserId,
            request.ReceivingUserId,
            request.Content
        );

        return Ok(message);
    }
}