using SocialBackend.tests.Data;
using SocialBackend.Models;
using SocialBackend.Services;
using Moq;
using Microsoft.AspNetCore.SignalR;
using SocialBackend.Hubs;
using SocialBackend.Exceptions;

namespace SocialBackend.tests.Services;

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
        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(async () =>
        {
            await _messageService.SendMessageAsync(_sendingUser.Id, _receivingUser.Id, "");
        });
    }

    [Fact]
    public async Task SendMessage_ShouldThrow_WhenSendingToSelf()
    {
        await Assert.ThrowsAsync<BadRequestException>(async () =>
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

    [Fact]
    public async Task GetConversationsAsync_ShouldReturnOneConversation_WithCorrectUnreadFlag()
    {
        await _messageService.SendMessageAsync(_sendingUser.Id, _receivingUser.Id, "Hello");
        await _messageService.SendMessageAsync(_receivingUser.Id, _sendingUser.Id, "Hi");

        var senderConvos = await _messageService.GetConversationsAsync(_sendingUser.Id);
        var receiverConvos = await _messageService.GetConversationsAsync(_receivingUser.Id);

        Assert.True(senderConvos.First().HasUnreadMessages);
        Assert.True(receiverConvos.First().HasUnreadMessages);
    }

    [Fact]
    public async Task GetConversationsAsync_ShouldReturnOneConversation_WhenAllMessagesRead()
    {
        await _messageService.SendMessageAsync(_sendingUser.Id, _receivingUser.Id, "Hello");
        await _messageService.SendMessageAsync(_receivingUser.Id, _sendingUser.Id, "Hi");

        await _messageService.MarkAsReadAsync(_sendingUser.Id, _receivingUser.Id);
        await _messageService.MarkAsReadAsync(_receivingUser.Id, _sendingUser.Id);

        var senderConvos = await _messageService.GetConversationsAsync(_sendingUser.Id);
        var receiverConvos = await _messageService.GetConversationsAsync(_receivingUser.Id);

        Assert.False(senderConvos.First().HasUnreadMessages);
        Assert.False(receiverConvos.First().HasUnreadMessages);
    }

    [Fact]
    public async Task GetConversationsAsync_ShouldNotDuplicateUsers()
    {
        await _messageService.SendMessageAsync(_sendingUser.Id, _receivingUser.Id, "Msg1");
        await _messageService.SendMessageAsync(_receivingUser.Id, _sendingUser.Id, "Msg2");

        var convos = await _messageService.GetConversationsAsync(_sendingUser.Id);

        Assert.Single(convos);
        Assert.Equal(_receivingUser.Id, convos.First().UserId);
    }

    [Fact]
    public async Task GetConversationsAsync_ShouldSortUnreadFirst_ThenByLastMessage()
    {
        var thirdUser = new User
        {
            Username = "third",
            Email = "third@example.com",
            PasswordHash = "Abcd1234!",
            Description = "I am the third user."
        };
        Context.Users.Add(thirdUser);
        Context.SaveChanges();

        await _messageService.SendMessageAsync(thirdUser.Id, _sendingUser.Id, "Unread");
        await _messageService.SendMessageAsync(_receivingUser.Id, _sendingUser.Id, "Old");
        await _messageService.MarkAsReadAsync(_sendingUser.Id, _receivingUser.Id);

        var convos = await _messageService.GetConversationsAsync(_sendingUser.Id);

        Assert.Equal(2, convos.Count);

        var first = convos[0];
        var second = convos[1];

        Assert.True(first.HasUnreadMessages);
        Assert.False(second.HasUnreadMessages);
    }

    [Fact]
    public async Task MarkAsReadAsync_ShouldSetIsReadTrue()
    {
        await _messageService.SendMessageAsync(_sendingUser.Id, _receivingUser.Id, "Unread");

        await _messageService.MarkAsReadAsync(_receivingUser.Id, _sendingUser.Id);

        var unread = Context.Messages
            .Where(m => m.SendingUserId == _sendingUser.Id && m.ReceivingUserId == _receivingUser.Id && !m.IsRead)
            .ToList();

        Assert.Empty(unread);
    }
}
