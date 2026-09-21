// ... existing usings ...
using System.Text;
using Gadema.Api.Services.Access.Authentication;
using Gadema.Api.Services.Base.Projects;
using Gadema.Api.Services.Search;
using Gadema.Api.Services.Tags.Strategies;
using Gadema.Api.Services.Writing.Characters;
using Gadema.Core.Interfaces;
using Gadema.Data.Database;
using Microsoft.OpenApi; 

namespace Gadema.Api;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Standard Services
        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor(); // Required for UserContext in the API layer

        // 2. Database Configuration
        builder.Services.AddDbContext<GameDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        // 3. Identity & Domain Services
        builder.Services.AddScoped<IIdentitySyncStrategy, ProjectIdentityStrategy>();
        builder.Services.AddScoped<IIdentitySyncStrategy, ContentIdentityStrategy>();
        builder.Services.AddDomainServices(); // This will now pick up the new UserContext in the API layer

        // 4. Search Configuration
        builder.Services.AddScoped<ISearchableProvider, ProjectService>();
        builder.Services.AddScoped<ISearchableProvider, CharacterService>(); 

        // 5. Authentication & Security
        builder.Services.AddSingleton<JwtTokenService>();
        builder.Services.AddScoped<EmailPasswordAuthService>();
        builder.Services.AddHttpClient("GoogleOAuth");

        var jwtSecret = builder.Configuration["Jwt:Secret"];
        if (string.IsNullOrWhiteSpace(jwtSecret) || Encoding.UTF8.GetByteCount(jwtSecret) < 32)
            throw new InvalidOperationException("Jwt:Secret must be set and at least 32 bytes long.");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret!)),
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "GaDeMa",
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"] ?? "GademaApi",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                };
            });

        builder.Services.AddAuthorization();

        // 6. Swagger Configuration (OpenAPI 3.1 compliant)
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "GaDeMa API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                        SecuritySchemeType = SecuritySchemeType.ApiKey,
                        In = ParameterLocation.Header
                    },
                    Array.Empty<string>()
                }
            });
        });

        var app = builder.Build();

        // 7. Middleware Pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(); // Enables the interactive UI at /swagger
        }
        else
        {
            app.UseExceptionHandler("/error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapGet("/health", () => Results.Ok(new { Status = "healthy" }));

        app.Run();
    }
}