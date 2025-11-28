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

        // Ignore SignalR 
        _mockHubContext
            .Setup(x => x.Clients.User(It.IsAny<string>()).SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object[]>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

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
        // Arrange
        var content = "TRE TVÅ ETT KÖR";

        // Act
        var result = await _messageService.SendMessageAsync(_sendingUser.Id, _receivingUser.Id, content);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(content, result.Content);
    }

    [Fact]
    public async Task SendMessage_ShouldThrowWhenContentIsEmpty()
    {
        // Arrange
        var content = "";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _messageService.SendMessageAsync(_sendingUser.Id, _receivingUser.Id, content);
        });
    }

    [Fact]
    public async Task GetMessagesBetweenUsers_ShouldReturnMessagesInChronologicalOrder()
    {
        // Arrange
        await _messageService.SendMessageAsync(_sendingUser.Id, _receivingUser.Id, "First");
        await _messageService.SendMessageAsync(_receivingUser.Id, _sendingUser.Id, "Second");
        await _messageService.SendMessageAsync(_sendingUser.Id, _receivingUser.Id, "Third");

        // Act
        var result = await _messageService.GetMessagesBetweenUsersAsync(
            _sendingUser.Id,
            _receivingUser.Id,
            pageIndex: 1,
            pageSize: 10
        );

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal("First", result.Items[0].Content);
        Assert.Equal("Second", result.Items[1].Content);
        Assert.Equal("Third", result.Items[2].Content);
    }

    [Fact]
    public async Task GetMessagesBetweenUsers_ShouldReturnEmptyList_WhenNoMessages()
    {
        var result = await _messageService.GetMessagesBetweenUsersAsync(
            _sendingUser.Id,
            _receivingUser.Id,
            pageIndex: 1,
            pageSize: 10
        );

        Assert.Empty(result.Items);
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
    public async Task GetMessagesBetweenUsers_ShouldThrowWhenInvalidPaginationParameters()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            await _messageService.GetMessagesBetweenUsersAsync(
                _sendingUser.Id,
                _receivingUser.Id,
                pageIndex: 0,
                pageSize: 10
            );
        });

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            await _messageService.GetMessagesBetweenUsersAsync(
                _sendingUser.Id,
                _receivingUser.Id,
                pageIndex: 1,
                pageSize: 0
            );
        });
    }

    [Fact]
    public async Task GetMessagesBetweenUsers_ShouldReturnCorrectPageOfMessages()
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

        // Act
        var result = await _messageService.GetMessagesBetweenUsersAsync(
            _sendingUser.Id,
            _receivingUser.Id,
            pageIndex: 2,
            pageSize: 10
        );

        // Assert
        Assert.Equal(10, result.Items.Count);
        Assert.Equal("Msg 11", result.Items.First().Content);
        Assert.Equal("Msg 20", result.Items.Last().Content);
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
        Assert.Equal(3, result.TotalPages);
    }
}