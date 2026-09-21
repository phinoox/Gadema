using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gadema.Api;

public static class AuthExtensions
{
    public static IServiceCollection AddGademaAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecret = configuration["Jwt:Secret"];
        if (string.IsNullOrWhiteSpace(jwtSecret) || Encoding.UTF8.GetByteCount(jwtSecret) < 32)
        {
            throw new InvalidOperationException("Jwt:Secret must be set and at least 32 bytes long.");
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret!)),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"] ?? "GaDeMa",
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"] ?? "GademaApi",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                };
            });

        services.AddAuthorization();
        return services;
    }
}