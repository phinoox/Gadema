using System.Text;
using Microsoft.AspNetCore.OpenApi;   
using Gadema.Api.Services.Access.Authentication;
using Gadema.Api.Services.Base.Projects;
using Gadema.Api.Services.Search;
using Gadema.Api.Services.Tags.Strategies;
using Gadema.Api.Services.Writing.Characters;
using Gadema.Core.Interfaces;
using Gadema.Data.Database;
using Scalar.AspNetCore;
using Gadema.Api.Services.Access;

namespace Gadema.Api;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Standard Services
        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();

        // 2. Database Configuration
        builder.Services.AddGademaData(builder.Configuration);
        
        // 3. Identity & Domain Services
        builder.Services.AddScoped<IIdentitySyncStrategy, ProjectIdentityStrategy>();
        builder.Services.AddScoped<IIdentitySyncStrategy, ContentIdentityStrategy>();
        builder.Services.AddDomainServices();

        // 4. Search Configuration
        builder.Services.AddScoped<IUserContext, UserContext>();
        builder.Services.AddScoped<ISearchableProvider, ProjectService>();
        builder.Services.AddScoped<ISearchableProvider, CharacterService>(); 

        // 5. Authentication & Security
        builder.Services.AddSingleton<JwtTokenService>();
        builder.Services.AddScoped<EmailPasswordAuthService>();
        builder.Services.AddHttpClient("GoogleOAuth");
        builder.Services.AddGademaAuthentication(builder.Configuration);

        // 6. OpenAPI Configuration (Native Microsoft implementation)
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // 7. Middleware Pipeline
        if (app.Environment.IsDevelopment())
        {
            // Map the OpenApi document generation endpoint
            app.MapOpenApi();
            // Map the Scalar UI for beautiful, interactive documentation
            app.MapScalarApiReference();
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