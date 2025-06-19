using System.Text;
using System.Text.Json;
using Chat.API.Hubs;
using Chat.API.Services;
using Chat.API.Utils;
using Chat.Business.Repositories;
using Chat.Business.Services;
using Chat.Core.Interfaces;
using Chat.Core.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Chat.API
{
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
                c.OperationFilter<FileUploadOperationFilter>();
                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
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
                            var accessToken = ctx.Request.Query["access_token"];
                            var path = ctx.HttpContext.Request.Path;
                            if (
                                !string.IsNullOrEmpty(accessToken)
                                && path.StartsWithSegments("/chatHub")
                            )
                                ctx.Token = accessToken;
                            return Task.CompletedTask;
                        },
                    };
                });

            services.AddCors(builder =>
                builder.AddPolicy(
                    "AllowAll",
                    policy =>
                        policy
                            .SetIsOriginAllowed(_ => true)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                )
            );

            services
                .AddSignalR(options => options.EnableDetailedErrors = true)
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.PropertyNamingPolicy =
                        JsonNamingPolicy.CamelCase;
                    options.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
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
            services.AddScoped<ChatService>();
            services.AddScoped<IUploadfileRepository, UploadfileRepository>();
            services.AddScoped<UploadfileService>();
        }

        public static void UseStartupMiddleware(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<ChatHub>("/chatHub");
        }
    }
}
