using FluentValidation.Results;
// Validators are declared in the global namespace in the main project, so no namespace import is required.
using SocialBackend.Dto;
using Xunit;

namespace social_backend.tests.Controller.Validator;

public class UserValidatorTests
{
    [Fact]
    public void LoginRequestValidator_ValidCredentials_PassesValidation()
    {
        // Arrange
        var validator = new LoginRequestValidator();
        var request = new LoginRequest { Username = "user1", Password = "password" };

        // Act
        ValidationResult result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void LoginRequestValidator_MissingUsername_FailsValidation()
    {
        // Arrange
        var validator = new LoginRequestValidator();
        var request = new LoginRequest { Username = string.Empty, Password = "password" };

        // Act
        ValidationResult result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Username");
    }

    [Fact]
    public void LoginRequestValidator_TooShortUsername_FailsValidation()
    {
        var validator = new LoginRequestValidator();
        var request = new LoginRequest { Username = "ab", Password = "password" };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("atleast three"));
    }

    [Fact]
    public void RegisterRequestValidator_ValidRequest_PassesValidation()
    {
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            Username = "validuser",
            Email = "test@example.com",
            Password = "Abcdef1!",
            Description = "A short user description"
        };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("bademail", false)]
    [InlineData("toolongemailaddress_toolong_toolong_toolong_toolong@example.com", false)]
    public void RegisterRequestValidator_InvalidEmail_FailsValidation(string email, bool expectedValid)
    {
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            Username = "validuser",
            Email = email,
            Password = "Abcdef1!",
            Description = "desc"
        };

        var result = validator.Validate(request);

        Assert.Equal(expectedValid, result.IsValid);
    }

    [Fact]
    public void RegisterRequestValidator_WeakPassword_FailsValidation()
    {
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            Username = "validuser",
            Email = "test@example.com",
            Password = "abcdef",
            Description = "desc"
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Fact]
    public void RegisterRequestValidator_DescriptionTooLong_FailsValidation()
    {
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            Username = "validuser",
            Email = "test@example.com",
            Password = "Abcdef1!",
            Description = new string('a', 301)
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }

    [Fact]
    public void UpdatePasswordRequestValidator_ValidPassword_PassesValidation()
    {
        var validator = new UpdatePasswordRequestValidator();
        var request = new UpdatePasswordRequest { NewPassword = "Abcdef1!" };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdatePasswordRequestValidator_WeakPassword_FailsValidation()
    {
        var validator = new UpdatePasswordRequestValidator();
        var request = new UpdatePasswordRequest { NewPassword = "abc" };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "NewPassword");
    }

    [Fact]
    public void UserIdRequestValidator_PositiveUserId_PassesValidation()
    {
        var validator = new UserIdRequestValidator();
        var request = new UserIdRequest { UserId = 1 };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void UserIdRequestValidator_NonPositiveUserId_FailsValidation()
    {
        var validator = new UserIdRequestValidator();
        var request = new UserIdRequest { UserId = 0 };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "UserId");
    }
}

