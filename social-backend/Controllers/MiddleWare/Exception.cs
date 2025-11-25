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
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        Console.WriteLine($"Middleware Error: {ex.Message}");

        // Todo now we sending back everything to client :)
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            statusCode = context.Response.StatusCode
        });

        return context.Response.WriteAsync(result);
    }
}
