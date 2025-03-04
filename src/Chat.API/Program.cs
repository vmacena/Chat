var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStartupServices();

var app = builder.Build();

app.UseStartupMiddleware();

app.Run();
