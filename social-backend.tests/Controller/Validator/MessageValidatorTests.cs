using FluentValidation.Results;
using SocialBackend.Dto;
using Xunit;

namespace social_backend.tests.Controller.Validator;

public class MessageValidatorTests
{
    [Fact]
    public void MessageDtoValidator_ValidMessage_PassesValidation()
    {
        // Arrange
        var validator = new MessageDtoValidator();
        var dto = new MessageDto(
            SendingUserId: 1,
            SendingUserName: "sender",
            ReceivingUserId: 2,
            ReceivingUserName: "receiver",
            CreatedAt: DateTime.UtcNow.AddSeconds(-1),
            Content: "Hello"
        );

        // Act
        ValidationResult result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void MessageDtoValidator_InvalidSendingUserId_FailsValidation(int sendingUserId)
    {
        var validator = new MessageDtoValidator();
        var dto = new MessageDto(
            SendingUserId: sendingUserId,
            SendingUserName: "sender",
            ReceivingUserId: 2,
            ReceivingUserName: "receiver",
            CreatedAt: DateTime.UtcNow.AddSeconds(-1),
            Content: "Hello"
        );

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "SendingUserId");
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("ab", false)]
    [InlineData("abc", true)]
    public void MessageDtoValidator_SendingUserNameLength_Validation(string name, bool expectedValid)
    {
        var validator = new MessageDtoValidator();
        var dto = new MessageDto(
            SendingUserId: 1,
            SendingUserName: name,
            ReceivingUserId: 2,
            ReceivingUserName: "receiver",
            CreatedAt: DateTime.UtcNow.AddSeconds(-1),
            Content: "Hello"
        );

        var result = validator.Validate(dto);

        Assert.Equal(expectedValid, result.IsValid);
    }

    [Fact]
    public void MessageDtoValidator_SendingUserNameTooLong_FailsValidation()
    {
        var validator = new MessageDtoValidator();
        var longName = new string('a', 51);
        var dto = new MessageDto(
            SendingUserId: 1,
            SendingUserName: longName,
            ReceivingUserId: 2,
            ReceivingUserName: "receiver",
            CreatedAt: DateTime.UtcNow.AddSeconds(-1),
            Content: "Hello"
        );

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "SendingUserName");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void MessageDtoValidator_InvalidReceivingUserId_FailsValidation(int receivingUserId)
    {
        var validator = new MessageDtoValidator();
        var dto = new MessageDto(
            SendingUserId: 1,
            SendingUserName: "sender",
            ReceivingUserId: receivingUserId,
            ReceivingUserName: "receiver",
            CreatedAt: DateTime.UtcNow.AddSeconds(-1),
            Content: "Hello"
        );

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ReceivingUserId");
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("ab", false)]
    [InlineData("abc", true)]
    public void MessageDtoValidator_ReceivingUserNameLength_Validation(string name, bool expectedValid)
    {
        var validator = new MessageDtoValidator();
        var dto = new MessageDto(
            SendingUserId: 1,
            SendingUserName: "sender",
            ReceivingUserId: 2,
            ReceivingUserName: name,
            CreatedAt: DateTime.UtcNow.AddSeconds(-1),
            Content: "Hello"
        );

        var result = validator.Validate(dto);

        Assert.Equal(expectedValid, result.IsValid);
    }

    [Fact]
    public void MessageDtoValidator_ReceivingUserNameTooLong_FailsValidation()
    {
        var validator = new MessageDtoValidator();
        var longName = new string('a', 51);
        var dto = new MessageDto(
            SendingUserId: 1,
            SendingUserName: "sender",
            ReceivingUserId: 2,
            ReceivingUserName: longName,
            CreatedAt: DateTime.UtcNow.AddSeconds(-1),
            Content: "Hello"
        );

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ReceivingUserName");
    }

    [Fact]
    public void MessageDtoValidator_CreatedAtDefault_FailsValidation()
    {
        var validator = new MessageDtoValidator();
        var dto = new MessageDto(
            SendingUserId: 1,
            SendingUserName: "sender",
            ReceivingUserId: 2,
            ReceivingUserName: "receiver",
            CreatedAt: default,
            Content: "Hello"
        );

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "CreatedAt");
    }

    [Fact]
    public void MessageDtoValidator_CreatedAtInFuture_FailsValidation()
    {
        var validator = new MessageDtoValidator();
        var dto = new MessageDto(
            SendingUserId: 1,
            SendingUserName: "sender",
            ReceivingUserId: 2,
            ReceivingUserName: "receiver",
            CreatedAt: DateTime.UtcNow.AddDays(1),
            Content: "Hello"
        );

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "CreatedAt");
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("Hello", true)]
    public void MessageDtoValidator_ContentValidation(string content, bool expectedValid)
    {
        var validator = new MessageDtoValidator();
        var dto = new MessageDto(
            SendingUserId: 1,
            SendingUserName: "sender",
            ReceivingUserId: 2,
            ReceivingUserName: "receiver",
            CreatedAt: DateTime.UtcNow.AddSeconds(-1),
            Content: content
        );

        var result = validator.Validate(dto);

        Assert.Equal(expectedValid, result.IsValid);
    }

    [Fact]
    public void MessageDtoValidator_ContentTooLong_FailsValidation()
    {
        var validator = new MessageDtoValidator();
        var longContent = new string('a', 301);
        var dto = new MessageDto(
            SendingUserId: 1,
            SendingUserName: "sender",
            ReceivingUserId: 2,
            ReceivingUserName: "receiver",
            CreatedAt: DateTime.UtcNow.AddSeconds(-1),
            Content: longContent
        );

        var result = validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Content");
    }

    [Fact]
    public void SendMessageRequestValidator_ValidRequest_PassesValidation()
    {
        var validator = new SendMessageRequestValidator();
        var req = new SendMessageRequest { ReceivingUserId = 2, Content = "Hi" };

        var result = validator.Validate(req);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void SendMessageRequestValidator_InvalidReceivingUserId_FailsValidation()
    {
        var validator = new SendMessageRequestValidator();
        var req = new SendMessageRequest { ReceivingUserId = 0, Content = "Hi" };

        var result = validator.Validate(req);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ReceivingUserId");
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("Hello", true)]
    public void SendMessageRequestValidator_ContentValidation(string content, bool expectedValid)
    {
        var validator = new SendMessageRequestValidator();
        var req = new SendMessageRequest { ReceivingUserId = 2, Content = content };

        var result = validator.Validate(req);

        Assert.Equal(expectedValid, result.IsValid);
    }

    [Fact]
    public void SendMessageRequestValidator_ContentTooLong_FailsValidation()
    {
        var validator = new SendMessageRequestValidator();
        var longContent = new string('a', 301);
        var req = new SendMessageRequest { ReceivingUserId = 2, Content = longContent };

        var result = validator.Validate(req);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Content");
    }
}
