var builder = WebApplication.CreateBuilder();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins("http://localhost:5173")
                .AllowCredentials();

        });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<IDatabaseContext, DatabaseContext>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IPasswordHelper, PasswordHelper>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, SessionUserIdProvider>();
builder.Services.AddDistributedMemoryCache(); // In memory session

// Sessioncookie
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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    db.Database.Migrate();
}

app.UseCors("AllowFrontend");
app.UseSession();
app.MapHub<ChatHub>("/chatHub");
app.UseMiddleware<ExceptionMiddleware>();
//app.UseHttpsRedirection();
//app.UseAuthorization(); todo?
app.MapControllers();

app.Run();
