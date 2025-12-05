namespace SocialBackend.Helpers;

public static class HttpContextExtensions
{
    public static int GetUserId(this HttpContext ctx)
    {
        var id = ctx.Session.GetInt32("UserId");
        if (id == null)
            throw new UnauthorizedAccessException("No user found in session");

        return id.Value;
    }
}