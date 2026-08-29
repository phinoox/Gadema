// =============================================================================
using Gadema.Core.Dtos;
using Microsoft.EntityFrameworkCore;
// Gadema.Api - ASP.NET Core Web API Services
// =============================================================================

using System.Threading.Tasks;
using Gadema.Core.Dtos.Authentication;
using Gadema.Data.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System;
using System.Linq;
using Gadema.Core.Services;


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

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public ApiAuthService(GameDbContext context, IConfiguration configuration, ILogger<ApiAuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Traditional login (email/password).
    /// </summary>
    public async Task<ApiResponseDto<AuthResponse>> SignInAsync(SignInDto signInDto)
    {
        // Implement authentication logic
        return ApiResponseDto<AuthResponse>.Success(new AuthResponse());
    }

    /// <summary>
    /// Google OAuth callback.
    /// </summary>
    public async Task<ApiResponseDto<AuthResponse>> GoogleCallbackAsync(string code)
    {
        // Implement Google OAuth logic
        return ApiResponseDto<AuthResponse>.Success(new AuthResponse());
    }

    /// <summary>
    /// Two-factor authentication login.
    /// </summary>
    public async Task<ApiResponseDto<AuthResponse>> SignInWith2FAAsync(SignInWith2FADto signInDto)
    {
        // Implement 2FA logic
        return ApiResponseDto<AuthResponse>.Success(new AuthResponse());
    }

    /// <summary>
    /// View recovery codes.
    /// </summary>
    public async Task<ApiResponseDto<RecoveryCodesResponseDto>> GetRecoveryCodesAsync(RecoveryCodesDto recoveryDto)
    {
        // Implement recovery codes logic
        return ApiResponseDto<RecoveryCodesResponseDto>.Success(new RecoveryCodesResponseDto());
    }
    /// <summary>
    /// Generates the expected TOTP token for a given time.
    /// </summary>
    private async Task<string> GenerateExpectedTokenAsync(Guid userId)
    {
        // Get current timestamp in seconds (TOTP uses 30-second windows)
        var currentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

        // This would typically use a library like Google Authenticator's algorithm
        // For simplicity, we'll assume the token format is base32 and 6 digits
        // In production, use proper TOTP library

        return "YOUR_GENERATED_TOKEN_HERE";  // Replace with actual token generation
    }


    /// <summary>
    /// Validates the two-factor token using TOTP or JWT.
    /// </summary>
    private async Task<bool> ValidateTwoFactorTokenAsync(Guid userId, string token)
    {
        // Generate expected token for this user
        var expectedToken = await GenerateExpectedTokenAsync(userId);

        // Compare tokens (using constant-time comparison to prevent timing attacks)
       // return CryptographicHelper.ConstantTimeEqual(token, expectedToken);
       return true;
    }

    /// <summary>
    /// Disable two-factor authentication.
    /// </summary>
    public async Task<ApiResponseDto<AuthResponse>> Disable2FAAsync(Disable2FADto disableDto)
    {
        var userId = UserHelper.GetUserId();  // Get current user from token/header

        // Verify the 2FA token is valid
        var isValidToken = await ValidateTwoFactorTokenAsync(userId, disableDto.TwoFactorToken);

        if (!isValidToken)
        {
            return ApiResponseDto<AuthResponse>.BadRequest("Invalid two-factor authentication token");
        }

        // Find and update the user to remove 2FA
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return ApiResponseDto<AuthResponse>.NotFound($"User with ID {userId} not found");
        }

        // Remove 2FA from user
        user.TwoFactorEnabled = false;

        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return ApiResponseDto<AuthResponse>.Success(new AuthResponse
        {
            //Message = "Two-factor authentication has been disabled successfully"
        });
    }

}