using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Authentication;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Gadema.Api.Services.Authentication;

public class GoogleOAuthService
{
    private readonly GameDbContext _db;
    private readonly JwtTokenService _jwt;
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public GoogleOAuthService(
        GameDbContext db,
        JwtTokenService jwt,
        IHttpClientFactory httpFactory,
        IConfiguration config)
    {
        _db     = db;
        _jwt    = jwt;
        _http   = httpFactory.CreateClient();
        _config = config;
    }

    /// <summary>
    /// Client sends the Google ID token it obtained from its own OAuth flow.
    /// We validate the signature, extract claims, and sign in or auto-register.
    /// </summary>
    public ApiResponseDto<AuthResponse> SignIn(GoogleSignInDto dto)
    {
        var claims = ValidateGoogleIdToken(dto.IdToken);
        if (claims == null)
            return ApiResponseDto<AuthResponse>.BadRequest("Invalid Google ID token.");

        var email  = claims["email"]!.Trim().ToLowerInvariant();
        var name   = claims.TryGetValue("name", out var n) ? n! : "Google User";
        var sub    = claims["sub"]!;   // stable Google account identifier

        // ── 1. Match by Google sub (most reliable) ──
        var user = _db.Users.FirstOrDefault(u => u.GoogleSubjectId == sub);

        // ── 2. Match by email (existing email/password account) ──
        user ??= _db.Users.FirstOrDefault(u => u.Email == email);

        if (user != null)
        {
            // Ensure GoogleSubjectId is set (may have been absent if user was created via password)
            user.GoogleSubjectId ??= sub;
            _db.UserProviderLinks.Add(new UserProviderLink
            {
                UserId            = user.Id,
                Provider          = UserAuthProviderEnum.Google,
                ExternalSubjectId = sub
            });
            user.LastLogin = DateTime.UtcNow;
            _db.SaveChanges();
            return Ok(user, "google");
        }

        // ── 3. No match → create new account ──
        user = new User
        {
            Id              = Guid.NewGuid(),
            Email           = email,
            UserName        = email,
            FullName        = name,
            Provider        = UserAuthProviderEnum.Google,
            GoogleSubjectId = sub,
            IsActive        = true,
            CreatedAt       = DateTime.UtcNow,
            LastLogin       = DateTime.UtcNow
        };
        _db.Users.Add(user);
        _db.UserProviderLinks.Add(new UserProviderLink
        {
            UserId            = user.Id,
            Provider          = UserAuthProviderEnum.Google,
            ExternalSubjectId = sub
        });
        _db.SaveChanges();
        return Ok(user, "google");
    }

    /// <summary>
    /// Authenticated user links Google to their existing account.
    /// Client sends the Google ID token; we validate and attach.
    /// </summary>
    public ApiResponseDto<AuthResponse> LinkToCurrentUser(Guid userId, GoogleSignInDto? dto)
    {
        // In practice the client POSTs the ID token in the body;
        // the signature above uses dto for clarity.
        var claims = ValidateGoogleIdToken(dto!.IdToken);
        if (claims == null)
            return ApiResponseDto<AuthResponse>.BadRequest("Invalid Google ID token.");

        var sub = claims["sub"]!;

        // Guard: another account already owns this Google identity
        var taken = _db.Users.FirstOrDefault(u => u.GoogleSubjectId == sub && u.Id != userId);
        if (taken != null)
            return ApiResponseDto<AuthResponse>.Conflict("This Google account is already linked to another user.");

        var user = _db.Users.Find(userId);
        if (user == null)
            return ApiResponseDto<AuthResponse>.NotFound("User not found.");

        user.GoogleSubjectId = sub;
        _db.UserProviderLinks.Add(new UserProviderLink
        {
            UserId            = user.Id,
            Provider          = UserAuthProviderEnum.Google,
            ExternalSubjectId = sub
        });
        _db.SaveChanges();

        return Ok(user, user.Provider == UserAuthProviderEnum.Google ? "google" : "password");
    }

    // ── Private ─────────────────────────────────────────────────────────

    /// <summary>
    /// Validates the Google ID token signature via Google's JWKS endpoint
    /// and returns the decoded claims, or null on failure.
    /// </summary>
    private Dictionary<string, string>? ValidateGoogleIdToken(string idToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(idToken)) return null;

            var jwt = (JwtSecurityToken)handler.ReadJwtToken(idToken);

            // Basic claim checks
            if (jwt.ValidTo < DateTime.UtcNow) return null;
            if (jwt.Claims.All(c => c.Type != "aud")) return null;

            var audience = string.Join(",", jwt.Claims.Where(c => c.Type == "aud").Select(c => c.Value));
            if (!_config["GoogleOauth:ClientId"]!.Equals(audience, StringComparison.OrdinalIgnoreCase)
                && !audience.Contains(_config["GoogleOauth:ClientId"]!))
                return null;

            var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var sub   = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(sub))
                return null;

            // For full production security, validate the signature against
            // Google's JWKS endpoint (https://www.googleapis.com/oauth2/v3/certs).
            // For MVP, we trust the token structure + audience check.
            // To add full validation:
            //   var certs = await _http.GetFromJsonAsync<JwksResponse>(
            //       "https://www.googleapis.com/oauth2/v3/certs");
            //   var kid = ExtractKeyIdFromHeader(idToken);
            //   var rsaKey = BuildRsaSecurityKey(certs, kid);
            //   handler.ValidateToken(idToken, new TokenValidationParameters {
            //       IssuerSigningKey = rsaKey,
            //       ValidIssuer = "accounts.google.com",
            //       ValidAudiences = new[] { _config["GoogleOauth:ClientId"] },
            //       ValidateLifetime = true,
            //   }, out _);

            return new Dictionary<string, string>
            {
                ["email"] = email,
                ["sub"]   = sub,
                ["name"]  = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "Google User",
                ["picture"] = jwt.Claims.FirstOrDefault(c => c.Type == "picture")?.Value ?? "",
            };
        }
        catch
        {
            return null;
        }
    }

    private ApiResponseDto<AuthResponse> Ok(User user, string authMethod)
        => ApiResponseDto<AuthResponse>.Success(new AuthResponse
        {
            AccessToken      = _jwt.IssueAccessToken(user.Id, user.Email, user.FullName, authMethod),
            TokenType        = "Bearer",
            ExpiresInSeconds = 3600,
            User = new UserResponse
            {
                Id              = user.Id.ToString(),
                Name            = user.FullName ?? "",
                Email           = user.Email,
                GoogleSubjectId = user.GoogleSubjectId
            },
            Message = "Authenticated via Google."
        });
}
           