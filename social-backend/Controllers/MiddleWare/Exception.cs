namespace SocialBackend.Controllers;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {RequestPath}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var (status, message) = MapExeptions(ex);

        context.Response.StatusCode = (int)status;

        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            statusCode = context.Response.StatusCode,
            error = message
        });

        return context.Response.WriteAsync(result);
    }

    private static (HttpStatusCode status, string message) MapExeptions(Exception ex)
    {
        return ex switch
        {
            NotFoundException => (HttpStatusCode.NotFound, ex.Message),
            BadRequestException => (HttpStatusCode.BadRequest, ex.Message),
            UnauthorizedException => (HttpStatusCode.Unauthorized, ex.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occured.")
        };
    }
}
