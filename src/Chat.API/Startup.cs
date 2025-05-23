// StartupExtensions.cs
using System.Text;
using System.Text.Json;
using Chat.API.Hubs;
using Chat.API.Services;
using Chat.Business.Repositories;
using Chat.Business.Services;
using Chat.Core.Interfaces;
using Chat.Core.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

public static class StartupExtensions
{
    public static void AddStartupServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chat API", Version = "v1" });
            c.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                }
            );
            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        },
                        Array.Empty<string>()
                    },
                }
            );
        });

        services.AddControllers();

        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);
        var issuer = configuration["Jwt:Issuer"]!;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                };
                opts.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        var token = ctx.Request.Query["access_token"];
                        var path = ctx.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(token) && path.StartsWithSegments("/chatHub"))
                            ctx.Token = token;
                        return Task.CompletedTask;
                    },
                };
            });

        services.AddCors(b =>
            b.AddPolicy(
                "AllowAll",
                p =>
                    p.SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
            )
        );

        services
            .AddSignalR()
            .AddJsonProtocol(o =>
            {
                o.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                o.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
            });

        services.AddDbContext<ChatDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection")!)
        );

        services.AddScoped<JwtService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<UserService>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<MessageService>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<ContactService>();
    }

    public static void UseStartupMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHub<ChatHub>("/chatHub");
    }
}
