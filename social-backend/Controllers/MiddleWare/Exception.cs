namespace SocialBackend.Controllers.MiddleWare;

public class ExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
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
            UserNotFoundException => (HttpStatusCode.NotFound, ex.Message),
            InvalidCredentialsException
            or UsernameExistsException
            or ArgumentOutOfRangeException
            or InvalidOperationException
            or CannotFollowSelfException
            or CannotUnfollowSelfException
            or AlreadyFollowingException
            or NotFollowingException => (HttpStatusCode.BadRequest, ex.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occured.")
        };
    }
}
