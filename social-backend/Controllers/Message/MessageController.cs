namespace SocialBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController(IMessageService messageService) : ControllerBase
{
    private readonly IMessageService _messageService = messageService;
    private int mockUserId = 2;

    [HttpPost]
    public async Task<ActionResult<MessageDto>> SendMessage([FromBody] SendMessageRequest request)
    {

        /* var currentUserId = HttpContext.GetUserId(); */
        var currentUserId = mockUserId;

        var message = await _messageService.SendMessageAsync(
            currentUserId,
            request.ReceivingUserId,
            request.Content
        );

        return Ok(message);
    }

    [HttpGet("{receivingUserId}")]
    public async Task<ActionResult<List<MessageDto>>> GetMessages(
        int receivingUserId,
        [FromQuery] DateTime? before = null)
    {
        var currentUserId = mockUserId;

        var messages = await _messageService.GetMessagesBetweenUsersAsync(
            currentUserId,
            receivingUserId,
            20,
            before
        );

        return Ok(messages);
    }

}