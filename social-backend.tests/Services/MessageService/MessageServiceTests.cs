using social_backend.tests.Data;
using SocialBackend.Models;
using Xunit;

public class MessageServiceTests : TestBase
{
    private readonly MessageService _messageService;
    public required User _sendingUser;
    public required User _receivingUser;

    public MessageServiceTests() : base()
    {
        _messageService = new MessageService(Context);
    }

    protected override void SeedData()
    {
        _sendingUser = new User { Username = "sender", Email = "sender@example.com", PasswordHash = "Abcd1234!" };
        _receivingUser = new User { Username = "receiver", Email = "receiver@example.com", PasswordHash = "Abcd1234!" };

        Context.Users.AddRange(_sendingUser, _receivingUser);
        Context.SaveChanges();
    }

    [Fact]
    public async Task SendMessage_ShouldReturnMessage()
    {
        // Arrange
        var content = "TRE TVÅ ETT KÖR";

        // Act
        var result = await _messageService.SendMessageAsync(_sendingUser.Id, _receivingUser.Id, content);

        // Assert
        Assert.NotNull(result);
    }

}