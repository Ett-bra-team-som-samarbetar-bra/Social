
var builder = WebApplication.CreateBuilder();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<IDatabaseContext, DatabaseContext>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddSingleton<Validator>();

//Adds in memory session
builder.Services.AddDistributedMemoryCache();

//Adds and configures sessioncookie
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".RootAccess.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.IdleTimeout = TimeSpan.FromHours(4);
});

// log level for EF Core >= Warning
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

var app = builder.Build();

// if (app.Environment.IsDevelopment()) {}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    db.Database.Migrate();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
//app.UseAuthorization(); todo?
app.MapControllers();
app.UseSession();

app.Run();
