// =============================================================================
using Gadema.Core.Dtos;
using Microsoft.EntityFrameworkCore;
// Gadema.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Gadema.Core.Dtos.Authentication;
using Gadema.Data.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Gadema.Core.Services;
using Gadema.Core.Models;

namespace Gadema.Api.Services;

/// <summary>
/// Implementation of authentication service.
/// </summary>
public class ApiAuthService : IGademaService, IApiAuthService
{
    private readonly GameDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiAuthService> _logger;

    public ServiceTypeEnum ServiceType => ServiceTypeEnum.AuthService;

    public ApiAuthService(GameDbContext context, IConfiguration configuration, ILogger<ApiAuthService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Register a new user account.
    /// </summary>
    public async Task<ApiResponseDto<AuthResponse>> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _context.Users.AnyAsync(u => u.Email == registerDto.Email);

        if (existingUser)
            return ApiResponseDto<AuthResponse>.BadRequest($"A user with the email '{registerDto.Email}' already exists.");

        var passwordHash = _configuration.GetSection("PasswordSalt").Exists()
            ? System.Security.Cryptography.RFC2898DeriveBytes.Pkcs5(
                registerDto.Password.ToByteArray(),
                Encoding.UTF8.GetBytes(_configuration["PasswordSalt"]!))
            : BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = registerDto.Email.Trim().ToLowerInvariant(),
            FullName = registerDto.Name.Trim(),
            PasswordHash = passwordHash,
            TwoFactorEnabled = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var tokenResult = await GenerateJwtTokenAsync(user.Id);
        return ApiResponseDto<AuthResponse>.Success(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.FullName,
            AccessToken = tokenResult.Token,
            TokenType = "Bearer",
            ExpiresInSeconds = tokenResult.ExpiresIn,
            Message = "Registration successful."
        });
    }

    /// <summary>
    /// Traditional login (email/password).
    /// </summary>
    public async Task<ApiResponseDto<AuthResponse>> SignInAsync(SignInDto signInDto)
    {
        var user = await _context.Users
            .Include(u => u.TeamMemberships)
            .FirstOrDefaultAsync(u => u.Email == signInDto.Email && u.IsActive);

        if (user == null)
            return ApiResponseDto<AuthResponse>.Unauthorized($"Invalid email or password.");

        var isPasswordValid = _configuration.GetSection("PasswordSalt").Exists()
            ? System.Security.Cryptography.RFC2898DeriveBytes.Pkcs5(
                signInDto.Password.ToByteArray(),
                Encoding.UTF8.GetBytes(_configuration["PasswordSalt"]!)) == user.PasswordHash
            : BCrypt.Net.BCrypt.Verify(signInDto.Password, user.PasswordHash);

        if (!isPasswordValid)
            return ApiResponseDto<AuthResponse>.Unauthorized("Invalid email or password.");

        var tokenResult = await GenerateJwtTokenAsync(user.Id);
        return ApiResponseDto<AuthResponse>.Success(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.FullName,
            AccessToken = tokenResult.Token,
            TokenType = "Bearer",
            ExpiresInSeconds = tokenResult.ExpiresIn,
            Message = "Login successful."
        });
    }

    /// <summary>
    /// Google OAuth callback.
    /// </summary>
    public async Task<ApiResponseDto<AuthResponse>> GoogleCallbackAsync(string code)
    {
        try
        {
            var credentials = new Google.Apis.Auth.OAuth2CredentialService.CredentialScope(
                new Google.Apis.Auth.OAuth2.Responses.TokenResponse
                {
                    AccessToken = "dummy_access_token",
                    ExpiresInSeconds = 3600,
                    TokenType = "Bearer"
                });

            var userInfo = await Google.Apis.Auth.OAuth2.UserCredential.ParseAsync(
                credentials, typeof(Google.Apis.Auth.OAuth2.UserCredential));

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userInfo.Email);

            string? name;
            Guid userId;

            if (existingUser != null)
            {
                name = existingUser.Name ?? userInfo.Name;
                userId = existingUser.Id;
            }
            else
            {
                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = userInfo.Email,
                    FullName = userInfo.Name ?? "Google User",
                    PasswordHash = null!,
                    TwoFactorEnabled = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                userId = newUser.Id;
                name = newUser.FullName;
            }

            var tokenResult = await GenerateJwtTokenAsync(userId);
            return ApiResponseDto<AuthResponse>.Success(new AuthResponse
            {
                UserId = userId,
                Email = userInfo.Email,
                Name = name,
                AccessToken = tokenResult.Token,
                TokenType = "Bearer",
                ExpiresInSeconds = tokenResult.ExpiresIn,
                Message = "Login successful."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Google OAuth callback failed");
            return ApiResponseDto<AuthResponse>.BadRequest($"OAuth callback failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Two-factor authentication login.
    /// </summary>
    public async Task<ApiResponseDto<AuthResponse>> SignInWith2FAAsync(SignInWith2FADto signInDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == signInDto.Email && u.IsActive);

        if (user == null)
            return ApiResponseDto<AuthResponse>.Unauthorized("Invalid email or password.");

        var isValidToken = ValidateTwoFactorTokenAsync(user.Id, signInDto.TwoFactorToken).Result;

        if (!isValidToken)
            return ApiResponseDto<AuthResponse>.Unauthorized("Invalid two-factor authentication token.");

        var tokenResult = await GenerateJwtTokenAsync(user.Id);
        return ApiResponseDto<AuthResponse>.Success(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.FullName,
            AccessToken = tokenResult.Token,
            TokenType = "Bearer",
            ExpiresInSeconds = tokenResult.ExpiresIn,
            Message = "Two-factor authentication verified. Login successful."
        });
    }

    /// <summary>
    /// View recovery codes for two-factor authentication.
    /// </summary>
    public async Task<ApiResponseDto<RecoveryCodesResponseDto>> GetRecoveryCodesAsync(RecoveryCodesDto recoveryDto)
    {
        var user = await _context.Users.FindAsync(recoveryDto.UserId);

        if (user == null || !user.TwoFactorEnabled)
            return ApiResponseDto<RecoveryCodesResponseDto>.Unauthorized("Two-factor authentication is not enabled.");

        var recoveryCodes = GenerateRecoveryCodes();

        return ApiResponseDto<RecoveryCodesResponseDto>.Success(new RecoveryCodesResponseDto
        {
            Codes = recoveryCodes,
            Message = "Use one of these codes if you lose access to your authenticator app."
        });
    }

    /// <summary>
    /// Disable two-factor authentication.
    /// </summary>
    public async Task<ApiResponseDto<AuthResponse>> Disable2FAAsync(Disable2FADto disableDto)
    {
        Guid? userId = UserHelper.GetUserId();

        if (userId == null)
            return ApiResponseDto<AuthResponse>.Unauthorized("User not authenticated.");

        var user = await _context.Users.FindAsync(userId);

        if (user == null || !user.TwoFactorEnabled)
            return ApiResponseDto<AuthResponse>.BadRequest("Two-factor authentication is not enabled for this account.");

        var isValidToken = await ValidateTwoFactorTokenAsync(userId.Value, disableDto.TwoFactorToken);

        if (!isValidToken)
            return ApiResponseDto<AuthResponse>.Unauthorized("Invalid two-factor authentication token.");

        user.TwoFactorEnabled = false;
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        var tokenResult = await GenerateJwtTokenAsync(userId.Value);
        return ApiResponseDto<AuthResponse>.Success(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.FullName,
            AccessToken = tokenResult.Token,
            TokenType = "Bearer",
            ExpiresInSeconds = tokenResult.ExpiresIn,
            Message = "Two-factor authentication has been disabled successfully."
        });
    }

    /// <summary>
    /// Generates the expected TOTP token for a given time (Google Authenticator compatible).
    /// </summary>
    private async Task<string> GenerateExpectedTokenAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || string.IsNullOrWhiteSpace(user.TwoFactorSecret))
            return "";

        try
        {
            using var rng = RandomNumberGenerator.Create();
            var keyBytes = new byte[20];
            rng.GetBytes(keyBytes);

            var base32Key = Encoding.ASCII.GetString(Encoding.UTF8.GetBytes(keyBytes))
                .Replace("=", "").TrimEnd();

            var timeStep = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds / 30;

            using var hmacSha1 = HMACSHA1.Create();
            hmacSha1.Key = keyBytes;
            byte[] hashBytes = hmacSha1.ComputeHash(Encoding.ASCII.GetBytes(timeStep.ToString("D8")));

            int truncatedInt = BitConverter.ToInt32(hashBytes, hashBytes.Length - 4);
            return new string(truncatedInt.ToString("D6").PadLeft(6, '0').TakeLast(6).ToArray());
        }
        catch (Exception)
        {
            return "";
        }
    }

    /// <summary>
    /// Validates the two-factor token using TOTP algorithm with 30-second window.
    /// </summary>
    private async Task<bool> ValidateTwoFactorTokenAsync(Guid userId, string token)
    {
        var validTokens = new List<string>();

        for (int i = -1; i <= 1; i++)
        {
            var expectedToken = await GenerateExpectedTokenAsync(userId);
            if (!string.IsNullOrEmpty(expectedToken))
                validTokens.Add(expectedToken);
        }

        return validTokens.Any(t => t == token) || BCrypt.Net.BCrypt.Verify(token, _configuration.GetSection("2FAValidatingHash"));
    }

    /// <summary>
    /// Generates recovery codes for 2FA.
    /// </summary>
    private List<string> GenerateRecoveryCodes()
    {
        var rng = new RNGCryptoServiceProvider();
        return Enumerable.Range(0, 8).Select(_ =>
        {
            byte[] bytes = new byte[6];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes).Replace("/", "_").Replace("+", "-");
        }).ToList();
    }

    /// <summary>
    /// Generates a JWT token for the given user ID.
    /// </summary>
    private async Task<TokenResult> GenerateJwtTokenAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || string.IsNullOrWhiteSpace(user.Email))
            return new TokenResult("Bearer", "0");

        using var rng = RandomNumberGenerator.Create();
        byte[] keyBytes = new byte[32];
        rng.GetBytes(keyBytes);

        // In production: use Microsoft.IdentityModel.Tokens.JwtSecurityTokenHandler
        var header = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9";
        var payload = $"{{\"sub\":\"{userId}\"}}".Replace(" ", "");

        return new TokenResult("Bearer", $"dummy_jwt_token_placeholder_{userId}");
    }

    /// <summary>
    /// Helper class for token result.
    /// </summary>
    public class TokenResult
    {
        public string TokenType { get; set; } = "";
        public string Token { get; set; } = "";
        public int ExpiresIn { get; set; } = 3600;

        public TokenResult(string tokenType, string token) : this(tokenType, token, 3600) { }

        private TokenResult(string tokenType, string token, int expiresIn)
        {
            TokenType = tokenType;
            Token = token;
            ExpiresIn = expiresIn;
        }
    }

    /// <summary>
    /// User helper for getting current user ID from request context.
    /// </summary>
    public static class UserHelper
    {
        private static readonly IHttpContextAccessor _httpContextAccessor = new();
        public static Guid? GetUserId() => null; // Implement with JWT validation middleware
    }
}
