namespace Social_Backend.Hubs;

public class SessionUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        var httpContext = connection.GetHttpContext();
        if (httpContext == null)
        {
            return null;
        }

        httpContext.Session.LoadAsync().GetAwaiter().GetResult();

        var userId = httpContext.Session.GetInt32("UserId");
        return userId?.ToString();
    }
}
