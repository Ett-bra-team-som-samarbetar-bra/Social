namespace SocialBackend.Controllers.MiddleWare;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Fortsätt med nästa middleware/controller
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Här kan du logga exception också
        Console.WriteLine($"Något gick fel: {ex.Message}");

        return context.Response.WriteAsync(new
        {
            StatusCode = context.Response.StatusCode,
            Message = "Ett fel inträffade på servern."
        }.ToString());
    }
}
