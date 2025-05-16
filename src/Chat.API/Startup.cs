using System.Text;
using Chat.API.Services;
using Chat.Business.Services;
using Chat.Core.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public static class StartupExtensions
{
    public static void AddStartupServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Swagger/OpenAPI
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Controllers
        services.AddControllers();

        // JWT Authentication
        services.AddScoped<JwtService>();
        var jwtKey = configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key");
        var jwtIssuer =
            configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer");
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    IssuerSigningKey = signingKey,
                };
            });

        // CORS
        services.AddCors(options =>
            options.AddPolicy(
                "AllowAll",
                builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            )
        );

        // DbContext
        services.AddDbContext<ChatDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")
                    ?? "Host=localhost;Port=5435;Database=postgres;Username=postgres;Password=root"
            )
        );

        // Business services
        services.AddScoped<UserService>();
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
    }
}
