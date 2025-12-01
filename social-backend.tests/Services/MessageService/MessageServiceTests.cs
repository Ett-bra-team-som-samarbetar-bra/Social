using social_backend.tests.Data;
using SocialBackend.Models;
using SocialBackend.Services;
using Xunit;
using Moq;
using Microsoft.AspNetCore.SignalR;
using Social_Backend.Hubs;

namespace social_backend.tests;

public class MessageServiceTests : TestBase
{
    private readonly IMessageService _messageService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IHubContext<ChatHub>> _mockHubContext;
    public User _sendingUser = null!;
    public User _receivingUser = null!;

    public MessageServiceTests() : base()
    {
        _mockUserService = new Mock<IUserService>();
        _mockHubContext = new Mock<IHubContext<ChatHub>>();

        // Mock for SignalR
        var mockClientProxy = new Mock<IClientProxy>();
        mockClientProxy
            .Setup(x => x.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object[]>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var mockClients = new Mock<IHubClients>();
        mockClients
            .Setup(x => x.User(It.IsAny<string>()))
            .Returns(mockClientProxy.Object);

        _mockHubContext
            .Setup(x => x.Clients)
            .Returns(mockClients.Object);

        _messageService = new MessageService(Context, _mockUserService.Object, _mockHubContext.Object);
    }

    protected override void SeedData()
    {
        _sendingUser = new User { Username = "sender", Email = "sender@example.com", PasswordHash = "Abcd1234!", Description = "I am the sender." };
        _receivingUser = new User { Username = "receiver", Email = "receiver@example.com", PasswordHash = "Abcd1234!", Description = "I am the receiver." };

        Context.Users.AddRange(_sendingUser, _receivingUser);
        Context.SaveChanges();
    }

    [Fact]
    public async Task SendMessage_ShouldReturnMessage()
    {
        var content = "TRE TVÅ ETT KÖR";

        var result = await _messageService.SendMessageAsync(_sendingUser.Id, _receivingUser.Id, content);

        Assert.NotNull(result);
        Assert.Equal(content, result.Content);
        Assert.Equal(_sendingUser.Id, result.SendingUserId);
        Assert.Equal(_receivingUser.Id, result.ReceivingUserId);
    }

    [Fact]
    public async Task SendMessage_ShouldThrowWhenContentIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _messageService.SendMessageAsync(_sendingUser.Id, _receivingUser.Id, "");
        });
    }

    [Fact]
    public async Task SendMessage_ShouldThrow_WhenSendingToSelf()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _messageService.SendMessageAsync(_sendingUser.Id, _sendingUser.Id, "Hello me");
        });
    }

    [Fact]
    public async Task GetMessagesBetweenUsers_ShouldReturnMessagesInChronologicalOrder()
    {
        await _messageService.SendMessageAsync(_sendingUser.Id, _receivingUser.Id, "First");
        await _messageService.SendMessageAsync(_receivingUser.Id, _sendingUser.Id, "Second");
        await _messageService.SendMessageAsync(_sendingUser.Id, _receivingUser.Id, "Third");

        var result = await _messageService.GetMessagesBetweenUsersAsync(
            _sendingUser.Id,
            _receivingUser.Id,
            pageSize: 20
        );

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("First", result[0].Content);
        Assert.Equal("Second", result[1].Content);
        Assert.Equal("Third", result[2].Content);
    }

    [Fact]
    public async Task GetMessagesBetweenUsers_ShouldReturnEmptyList_WhenNoMessages()
    {
        var result = await _messageService.GetMessagesBetweenUsersAsync(
            _sendingUser.Id,
            _receivingUser.Id,
            pageSize: 20
        );

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetMessagesBetweenUsers_ShouldReturnCorrectMessagesUsingBeforeCursor()
    {
        // Arrange 
        for (int i = 1; i <= 30; i++)
        {
            await _messageService.SendMessageAsync(
                _sendingUser.Id,
                _receivingUser.Id,
                $"Msg {i}"
            );
        }

        var firstPage = await _messageService.GetMessagesBetweenUsersAsync(
            _sendingUser.Id,
            _receivingUser.Id,
            pageSize: 20
        );

        Assert.Equal(20, firstPage.Count);
        Assert.Equal("Msg 11", firstPage.First().Content); 
        Assert.Equal("Msg 30", firstPage.Last().Content);

        var before = firstPage.First().CreatedAt;

        var secondPage = await _messageService.GetMessagesBetweenUsersAsync(
            _sendingUser.Id,
            _receivingUser.Id,
            pageSize: 20,
            before: before
        );

        Assert.Equal(10, secondPage.Count);
        Assert.Equal("Msg 1", secondPage.First().Content);
        Assert.Equal("Msg 10", secondPage.Last().Content);
    }
}
