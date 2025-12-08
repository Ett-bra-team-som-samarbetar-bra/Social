using Microsoft.AspNetCore.Authorization;

namespace SocialBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController(IMessageService messageService) : ControllerBase
{
    private readonly IMessageService _messageService = messageService;

    [Authorize]
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

    [Authorize]
    [HttpGet("{receivingUserId}")]
    public async Task<ActionResult<List<MessageDto>>> GetMessages(
     int receivingUserId,
     [FromQuery] DateTime? before = null)
    {
        var currentUserId = HttpContext.GetUserId();

        await _messageService.MarkAsReadAsync(currentUserId, receivingUserId);

        var messages = await _messageService.GetMessagesBetweenUsersAsync(
            currentUserId,
            receivingUserId,
            20,
            before
        );

        return Ok(messages);
    }

    [Authorize]
    [HttpGet("conversations")]
    public async Task<ActionResult<List<ConversationDto>>> GetConversations()
    {
        var currentUserId = HttpContext.GetUserId();

        var conversations = await _messageService.GetConversationsAsync(currentUserId);

        return Ok(conversations);
    }

    [Authorize]
    [HttpPost("{otherUserId}/read")]
    public async Task<IActionResult> MarkAsRead(int otherUserId)
    {
        var currentUserId = HttpContext.GetUserId();

        await _messageService.MarkAsReadAsync(currentUserId, otherUserId);

        return Ok();
    }
}