using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SocialBackend.Controllers.MiddleWare;
using SocialBackend.Exceptions;
using Xunit;

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

    [Fact]
    public async Task UserNotFoundException_Returns_NotFound()
    {
        var ex = new UserNotFoundException(42);

        var (status, contentType, json) = await RunMiddlewareWithExceptionAsync(ex);

        Assert.Equal((int)HttpStatusCode.NotFound, status);
        Assert.Equal("application/json", contentType);
        Assert.Equal((int)status, json.GetProperty("statusCode").GetInt32());
        Assert.Equal(ex.Message, json.GetProperty("error").GetString());
    }

    [Fact]
    public async Task InvalidCredentialsException_Returns_BadRequest()
    {
        var ex = new InvalidCredentialsException();

        var (status, contentType, json) = await RunMiddlewareWithExceptionAsync(ex);

        Assert.Equal((int)HttpStatusCode.BadRequest, status);
        Assert.Equal("application/json", contentType);
        Assert.Equal((int)status, json.GetProperty("statusCode").GetInt32());
        Assert.Equal(ex.Message, json.GetProperty("error").GetString());
    }

    [Fact]
    public async Task UsernameExistsException_Returns_BadRequest()
    {
        var ex = new UsernameExistsException();

        var (status, contentType, json) = await RunMiddlewareWithExceptionAsync(ex);

        Assert.Equal((int)HttpStatusCode.BadRequest, status);
        Assert.Equal("application/json", contentType);
        Assert.Equal((int)status, json.GetProperty("statusCode").GetInt32());
        Assert.Equal(ex.Message, json.GetProperty("error").GetString());
    }

    [Fact]
    public async Task ArgumentOutOfRangeException_Returns_BadRequest()
    {
        var ex = new ArgumentOutOfRangeException("param", "out of range");

        var (status, contentType, json) = await RunMiddlewareWithExceptionAsync(ex);

        Assert.Equal((int)HttpStatusCode.BadRequest, status);
        Assert.Equal("application/json", contentType);
        Assert.Equal((int)status, json.GetProperty("statusCode").GetInt32());
        Assert.Equal(ex.Message, json.GetProperty("error").GetString());
    }

    [Fact]
    public async Task InvalidOperationException_Returns_BadRequest()
    {
        var ex = new InvalidOperationException("invalid operation");

        var (status, contentType, json) = await RunMiddlewareWithExceptionAsync(ex);

        Assert.Equal((int)HttpStatusCode.BadRequest, status);
        Assert.Equal("application/json", contentType);
        Assert.Equal((int)status, json.GetProperty("statusCode").GetInt32());
        Assert.Equal(ex.Message, json.GetProperty("error").GetString());
    }

    [Fact]
    public async Task UnknownException_Returns_InternalServerError_WithGenericMessage()
    {
        var ex = new Exception("boom");

        var (status, contentType, json) = await RunMiddlewareWithExceptionAsync(ex);

        Assert.Equal((int)HttpStatusCode.InternalServerError, status);
        Assert.Equal("application/json", contentType);
        Assert.Equal((int)status, json.GetProperty("statusCode").GetInt32());
        Assert.Equal("An unexpected error occured.", json.GetProperty("error").GetString());
    }
}
