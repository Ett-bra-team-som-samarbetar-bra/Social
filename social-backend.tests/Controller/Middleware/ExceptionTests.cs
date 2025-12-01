using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SocialBackend.Controllers.MiddleWare;
using SocialBackend.Exceptions;
using Xunit;
using static System.Net.HttpStatusCode;

namespace social_backend.tests.Controller.MiddleWare;

public class ExceptionTests
{
    private static async Task<(int statusCode, string contentType, JsonElement body)> RunMiddlewareWithExceptionAsync(Exception ex)
    {
        RequestDelegate next = _ => Task.FromException(ex);
        var middleware = new ExceptionMiddleware(next);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var sr = new StreamReader(context.Response.Body);
        var bodyString = await sr.ReadToEndAsync();
        var json = JsonDocument.Parse(bodyString).RootElement;

        return (context.Response.StatusCode, context.Response.ContentType ?? string.Empty, json);
    }

    private static void AssertExceptionResponse(int statusCode, string contentType, JsonElement json, int expectedStatus, string expectedError)
    {
        Assert.Equal(expectedStatus, statusCode);
        Assert.Equal("application/json", contentType);
        Assert.Equal(expectedStatus, json.GetProperty("statusCode").GetInt32());
        Assert.Equal(expectedError, json.GetProperty("error").GetString());
    }

    [Fact]
    public async Task UserNotFoundException_Returns_NotFound()
    {
        var ex = new UserNotFoundException(42);
        var (status, contentType, json) = await RunMiddlewareWithExceptionAsync(ex);

        AssertExceptionResponse(status, contentType, json, (int)HttpStatusCode.NotFound, ex.Message);
    }

    [Fact]
    public async Task InvalidCredentialsException_Returns_BadRequest()
    {
        var ex = new InvalidCredentialsException();
        var (status, contentType, json) = await RunMiddlewareWithExceptionAsync(ex);

        AssertExceptionResponse(status, contentType, json, (int)HttpStatusCode.BadRequest, ex.Message);
    }

    [Fact]
    public async Task UsernameExistsException_Returns_BadRequest()
    {
        var ex = new UsernameExistsException();
        var (status, contentType, json) = await RunMiddlewareWithExceptionAsync(ex);

        AssertExceptionResponse(status, contentType, json, (int)HttpStatusCode.BadRequest, ex.Message);
    }

    [Fact]
    public async Task ArgumentOutOfRangeException_Returns_BadRequest()
    {
        var ex = new ArgumentOutOfRangeException("param", "out of range");
        var (status, contentType, json) = await RunMiddlewareWithExceptionAsync(ex);

        AssertExceptionResponse(status, contentType, json, (int)HttpStatusCode.BadRequest, ex.Message);
    }

    [Fact]
    public async Task InvalidOperationException_Returns_BadRequest()
    {
        var ex = new InvalidOperationException("invalid operation");
        var (status, contentType, json) = await RunMiddlewareWithExceptionAsync(ex);

        AssertExceptionResponse(status, contentType, json, (int)HttpStatusCode.BadRequest, ex.Message);
    }

    [Fact]
    public async Task CannotFollowSelfException_Returns_BadRequest()
    {
        var ex = new CannotFollowSelfException();
        var (status, contentType, json) = await RunMiddlewareWithExceptionAsync(ex);

        AssertExceptionResponse(status, contentType, json, (int)HttpStatusCode.BadRequest, ex.Message);
    }

    [Fact]
    public async Task CannotUnfollowSelfException_Returns_BadRequest()
    {
        var ex = new CannotUnfollowSelfException();
        var (status, contentType, json) = await RunMiddlewareWithExceptionAsync(ex);

        AssertExceptionResponse(status, contentType, json, (int)HttpStatusCode.BadRequest, ex.Message);
    }

    [Fact]
    public async Task AlreadyFollowingException_Returns_BadRequest()
    {
        var ex = new AlreadyFollowingException();
        var (status, contentType, json) = await RunMiddlewareWithExceptionAsync(ex);

        AssertExceptionResponse(status, contentType, json, (int)HttpStatusCode.BadRequest, ex.Message);
    }

    [Fact]
    public async Task NotFollowingException_Returns_BadRequest()
    {
        var ex = new NotFollowingException();
        var (status, contentType, json) = await RunMiddlewareWithExceptionAsync(ex);

        AssertExceptionResponse(status, contentType, json, (int)HttpStatusCode.BadRequest, ex.Message);
    }

    [Fact]
    public async Task UnknownException_Returns_InternalServerError_WithGenericMessage()
    {
        var ex = new Exception("boom");
        var (status, contentType, json) = await RunMiddlewareWithExceptionAsync(ex);

        AssertExceptionResponse(status, contentType, json, (int)HttpStatusCode.InternalServerError, "An unexpected error occured.");
    }
}
