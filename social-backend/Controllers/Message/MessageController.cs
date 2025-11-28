namespace SocialBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController(IMessageService messageService) : ControllerBase
{
    private readonly IMessageService _messageService = messageService;

    [HttpPost]
    public async Task<ActionResult<MessageDto>> SendMessage([FromBody] SendMessageRequest request)
    {
        var currentUserId = HttpContext.GetUserId();

        var message = await _messageService.SendMessageAsync(
            currentUserId,
            request.ReceivingUserId,
            request.Content
        );

        return Ok(message);
    }

    [HttpGet("{receivingUserId}")]
    public async Task<ActionResult<PaginatedList<MessageDto>>> GetMessages(int receivingUserId, int pageIndex = 1)
    {
        var currentUserId = HttpContext.GetUserId();

        var messages = await _messageService.GetMessagesBetweenUsersAsync(
            currentUserId,
            receivingUserId,
            pageIndex,
            20
        );

        return Ok(messages);
    }
}