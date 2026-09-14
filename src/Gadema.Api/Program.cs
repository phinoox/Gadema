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
using System.Security.Claims;
using Gadema.Api.Services.Content;
using Gadema.Api.Services.Projects;
using Gadema.Api.Services.Tasks;
using Gadema.Api.Services.Narrative;
using Gadema.Api.Services.DialogueTrees;
using Gadema.Core.Interfaces.Identity;
using Gadema.Api.Services.Tags.Strategies;
using Gadema.Api.Services.Tags;
using Gadema.Api.Services.Search;
using Gadema.Api.Services.Characters;

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

        // --- Register Identity Sync Strategies ---
        builder.Services.AddScoped<IIdentitySyncStrategy, ProjectIdentityStrategy>();
        builder.Services.AddScoped<IIdentitySyncStrategy, ContentIdentityStrategy>();

        // Register service implementations
        builder.Services.AddScoped<ProjectService, ProjectService>();
        builder.Services.AddScoped<DialogueBranchService, DialogueBranchService>();
        builder.Services.AddScoped<ProjectTaskService, ProjectTaskService>();
        builder.Services.AddScoped<CommentService, CommentService>();
        builder.Services.AddScoped<ExternalReferenceService, ExternalReferenceService>();
        builder.Services.AddScoped<StoryOutlineService, StoryOutlineService>();
        builder.Services.AddScoped<ReviewStatusService, ReviewStatusService>();
        builder.Services.AddScoped<IUserContext, UserContext>();
        builder.Services.AddScoped<MetaTagService>(); // Previously TagService
        builder.Services.AddScoped<ProjectService>();

        //search
        builder.Services.AddScoped<ISearchableProvider, ProjectService>(); // Already implemented
        builder.Services.AddScoped<ISearchableProvider, CharacterService>(); 

        // ── Authentication ──────────────────────────────────────────────────
        builder.Services.AddSingleton<JwtTokenService>();
        builder.Services.AddScoped<EmailPasswordAuthService>();

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
            //   app.UseHttpsRedirection();
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