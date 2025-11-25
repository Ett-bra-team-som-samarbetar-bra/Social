var builder = WebApplication.CreateBuilder();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ISocialContext, SocialContext>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IPostService, PostService>();
//builder.Services.AddSingleton<Validator>();

// log level for EF Core >= Warning
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SocialContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
//app.UseAuthorization();
app.MapControllers();

app.Run();
