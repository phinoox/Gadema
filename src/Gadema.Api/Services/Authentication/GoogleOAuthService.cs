// ========================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Authentication;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Authentication;

/// <summary>
/// Service for Google OAuth authentication.
/// Handles token exchange, user creation, and provider linking.
/// </summary>
public class GoogleOAuthService : CoreService
{
    private readonly IConfiguration _configuration;

    public GoogleOAuthService(GameDbContext db, ILogger<GoogleOAuthService> logger, IUserContext userContext, IConfiguration configuration)
        : base(db, logger, userContext) => _configuration = configuration;

    // ========================================================================
    // POST /api/v1/auth/google/callback - Exchange Google token for session
    // ========================================================================

    public async Task<ApiResponseDto<AuthResponse>> CallbackAsync(GoogleCallbackDto callbackDto)
    {
        var credentials = new ClientSecretCredential("https://www.googleapis.com/auth/userinfo.profile", _configuration["Google:ClientId"]!, _configuration["Google:ClientSecret"]!);
        var tokenResult = await credentials.GetTokenAsync(new TokenRequestContext(new[] { "openid", "profile", "email" }));

        if (tokenResult.AccessToken is null)
            return ApiResponseDto<AuthResponse>.BadRequest("Failed to exchange Google token.");

        // Fetch user info from Google API
        var userInfo = await FetchGoogleUserInfo(tokenResult.AccessToken);

        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == userInfo.Email);

        if (existingUser is null)
        {
            // Create new user
            existingUser = new User
            {
                Id = Guid.NewGuid(),
                Email = userInfo.Email,
                Name = $"{userInfo.GivenName} {userInfo.FamilyName}",
                PictureUrl = userInfo.PictureUrl,
                IsTeamMemberOnly = false, // Google users are not team-member-restricted
            };

            _db.Users.Add(existingUser);
        }

        if (existingUser.Id != Guid.Empty)
        {
            // Link to Google provider
            var existingLink = await _db.UserProviderLinks.FirstOrDefaultAsync(l => l.UserId == existingUser.Id && l.ProviderType == "Google");
            if (existingLink is null)
            {
                _db.UserProviderLinks.Add(new UserProviderLink
                {
                    Id = Guid.NewGuid(),
                    UserId = existingUser.Id,
                    ProviderType = "Google",
                    ProviderUserId = userInfo.Id
                });
                await _db.SaveChangesAsync();
            }
        }
    }

    // ========================================================================
    // Helpers
    // ========================================================================

    private async Task<GoogleUserInfo> FetchGoogleUserInfo(string accessToken)
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo",
            new System.Net.Http.Headers.ProductInfoHeaderValue("Gadema", "1.0"));

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GoogleUserInfo>(content)!;
        }

        throw new HttpRequestException($"Failed to fetch Google user info: {response.StatusCode}");
    }
}

public record GoogleUserInfo(string Id, string Email, string GivenName, string FamilyName, string PictureUrl);
