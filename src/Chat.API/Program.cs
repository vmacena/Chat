using Chat.API;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddStartupServices(builder.Configuration);
var app = builder.Build();
app.UseStartupMiddleware();
app.Run();
