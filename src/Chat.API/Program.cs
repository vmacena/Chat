using Chat.Core.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddStartupServices(builder.Configuration);

var app = builder.Build();

app.UseStartupMiddleware();

app.Run();
