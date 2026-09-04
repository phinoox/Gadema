// =============================================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gadema.Core.Dtos;
using Gadema.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Http;
using Gadema.Data.Database;
using Gadema.Core.Services;
using Gadema.Api.Services.Authentication;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Gadema.Api;

// Gadema.Api - ASP.NET Core Web API Entry Point
// =============================================================================

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddControllers();
      
        // if(builder.Environment.IsProduction() || builder.Environment.IsDevelopment())
        {
            builder.Services.AddDbContext<GameDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        }

        // Register service implementations
        builder.Services.AddScoped<IContentService, ContentItemService>();
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<IDialogueService, DialogueService>();
        builder.Services.AddScoped<IProjectTaskService, ProjectTaskService>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<IExternalReferenceService, ExternalReferenceService>();
        builder.Services.AddScoped<IStoryOutlineService, StoryOutlineService>();
        builder.Services.AddScoped<IExportService, ExportService>();
        builder.Services.AddScoped<ITagService, TagService>();
        builder.Services.AddScoped<IReviewStatusService, ReviewStatusService>();
        builder.Services.AddScoped<IUserContext, UserContext>();

        // ── Authentication ──────────────────────────────────────────────────
        builder.Services.AddSingleton<JwtTokenService>();
        builder.Services.AddScoped<EmailPasswordAuthService>();
        builder.Services.AddScoped<TwoFactorAuthService>();
        builder.Services.AddScoped<GoogleOAuthService>();

        // HttpClient for Google JWKS (or other outbound calls)
        builder.Services.AddHttpClient("GoogleOAuth");
        builder.Services.AddHttpContextAccessor();

        var jwtSecret = builder.Configuration["Jwt:Secret"];
        if (string.IsNullOrWhiteSpace(jwtSecret) || Encoding.UTF8.GetByteCount(jwtSecret) < 32)
            throw new InvalidOperationException("Jwt:Secret must be set and at least 32 bytes long.");

        // JWT Bearer middleware – protects /2fa/* and /google/link endpoints
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "GaDeMa",
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"] ?? "GaDeMaApi",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                };
            });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
        {
            app.UseHttpsRedirection();
            app.UseExceptionHandler("/error");
            app.MapGet("/error", () => Results.Problem(detail: "An unexpected error occurred.", statusCode: StatusCodes.Status500InternalServerError));
            app.UseHsts();
        }

        
        app.UseAuthentication(); 
        app.UseAuthorization();
        app.MapControllers();
        app.MapGet("/health", () => Results.Ok(new { Status = "healthy" }));
        app.Run();

    }
}