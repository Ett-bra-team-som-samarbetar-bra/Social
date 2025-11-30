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

        var (status, message) = ex switch
        {
            UserNotFoundException => (HttpStatusCode.NotFound, ex.Message),
            InvalidCredentialsException => (HttpStatusCode.BadRequest, ex.Message),
            UsernameExistsException => (HttpStatusCode.BadRequest, ex.Message),
            ArgumentOutOfRangeException => (HttpStatusCode.BadRequest, ex.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, ex.Message),
            CannotFollowSelfException => (HttpStatusCode.BadRequest, ex.Message),
            CannotUnfollowSelfException => (HttpStatusCode.BadRequest, ex.Message),
            AlreadyFollowingException => (HttpStatusCode.BadRequest, ex.Message),
            NotFollowingException => (HttpStatusCode.BadRequest, ex.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occured.")
        };

        context.Response.StatusCode = (int)status;

        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            statusCode = context.Response.StatusCode,
            error = message
        });

        return context.Response.WriteAsync(result);
    }
}
