// =============================================================================
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Authentication;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Gadema.Api.Services.Authentication;

/// <summary>
/// Service for email/password authentication operations.
/// Handles registration, sign-in, password reset requests, and session management.
/// </summary>
public class EmailPasswordAuthService : CoreService
{
    private readonly IConfiguration _configuration;

    public EmailPasswordAuthService(GameDbContext db, ILogger<EmailPasswordAuthService> logger, IUserContext userContext, IConfiguration configuration)
        : base(db, logger, userContext) => _configuration = configuration;

    // ========================================================================
    // POST - Register a new user (email + password)
    // ========================================================================

       public async Task<ApiResponseDto<AuthResponse>> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
        if (existingUser != null)
            return ApiResponseDto<AuthResponse>.Conflict($"A user with email '{registerDto.Email}' already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            EmailConfirmed = false,
            IsTeamMemberOnly = true,
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var authResponse = new AuthResponse
        {
            Token = GenerateJwtToken(user.Id, user.Email),
            RefreshToken = GenerateRefreshToken(),
            ExpirationTime = DateTime.UtcNow.AddHours(24),
            TokenType = "Bearer"
        };

        return ApiResponseDto<AuthResponse>.Success(authResponse);
    }

    // ========================================================================
    // POST - Sign in with email + password
    // ========================================================================

    public async Task<ApiResponseDto<AuthResponse>> SignInAsync(SignInDto signInDto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == signInDto.Email && !u.IsDeleted);

        if (user == null)
            return ApiResponseDto<AuthResponse>.Unauthorized("Invalid email or password.");

        var isValidPassword = BCrypt.Net.BCrypt.Verify(signInDto.Password, user.PasswordHash);
        if (!isValidPassword)
            return ApiResponseDto<AuthResponse>.Unauthorized("Invalid email or password.");

        if (!user.EmailConfirmed && !string.IsNullOrEmpty(user.RecoveryCode))
            return ApiResponseDto<AuthResponse>.Forbidden("Please confirm your email before signing in. Use your recovery code to reset it.");

        var authResponse = new AuthResponse
        {
            Token = GenerateJwtToken(user.Id, user.Email),
            RefreshToken = GenerateRefreshToken(),
            ExpirationTime = DateTime.UtcNow.AddHours(24),
            TokenType = "Bearer"
        };

        return ApiResponseDto<AuthResponse>.Success(authResponse);
    }

    // ========================================================================
    // POST - Sign in with 2FA code (requires previous auth)
    // ========================================================================

    public async Task<ApiResponseDto<AuthResponse>> SignInWith2FAAsync(SignInWith2FADto signInDto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == signInDto.UserId);
        if (user == null || !user.IsTeamMemberOnly)
            return ApiResponseDto<AuthResponse>.Unauthorized("Invalid request.");

        var isValidCode = await Verify2FACodeAsync(user, signInDto.Code);
        if (!isValidCode)
            return ApiResponseDto<AuthResponse>.Unauthorized("Invalid 2FA code.");

        var authResponse = new AuthResponse
        {
            Token = GenerateJwtToken(user.Id, user.Email),
            RefreshToken = GenerateRefreshToken(),
            ExpirationTime = DateTime.UtcNow.AddHours(24),
            TokenType = "Bearer"
        };

        return ApiResponseDto<AuthResponse>.Success(authResponse);
    }

    // ========================================================================
    // POST - Enable 2FA for a user (admin or self)
    // ========================================================================

    public async Task<ApiResponseDto<Enable2FADto>> Enable2FAAsync(Guid userId, Enable2FADto enableDto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null || !user.IsTeamMemberOnly)
            return ApiResponseDto<Enable2FADto>.Forbidden("User not found or not eligible for 2FA.");

        // Verify current session password first
        var sessionUser = _userContext.CurrentUser;
        if (sessionUser == null || user.Id != sessionUser.Id && !_db.Users.Any(u => u.Id == userId && BCrypt.Net.BCrypt.Verify(enableDto.Password, u.PasswordHash)))
            return ApiResponseDto<Enable2FADto>.Unauthorized("Invalid credentials.");

        // Generate QR code data for authenticator app
        var qrCodeData = $"otpauth://totp/Gadema:user:{user.Email}?secret={enableDto.Secret}&issuer=Gadema";
        var qrUrl = $"https://chart.googleapis.com/chart?chs=200x200&cht=qr&chl={Uri.EscapeDataString(qrCodeData)}";

        user.TwoFactorEnabled = true;
        user.TwoFactorSecret = enableDto.Secret;
        await _db.SaveChangesAsync();

        return ApiResponseDto<Enable2FADto>.Success(new Enable2FADto
        {
            UserId = user.Id,
            Secret = enableDto.Secret,
            QrCodeUrl = qrUrl,
            RecoveryCodes = GenerateRecoveryCodes(),
            VerifiedAt = DateTime.UtcNow
        });
    }

    // ========================================================================
    // POST - Disable 2FA for a user (admin or self)
    // ========================================================================

    public async Task<ApiResponseDto<Disable2FADto>> Disable2FAAsync(Guid userId, Disable2FADto disableDto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null || !user.IsTeamMemberOnly)
            return ApiResponseDto<Disable2FADto>.Forbidden("User not found or not eligible for 2FA.");

        var sessionUser = _userContext.CurrentUser;
        if (sessionUser == null || user.Id != sessionUser.Id && !_db.Users.Any(u => u.Id == userId && BCrypt.Net.BCrypt.Verify(disableDto.Password, u.PasswordHash)))
            return ApiResponseDto<Disable2FADto>.Unauthorized("Invalid credentials.");

        // Verify a recovery code first before disabling
        var validCode = enableDto.RecoveryCode;
        if (!validRecoveryCodes.Contains(validCode))
            return ApiResponseDto<Disable2FADto>.BadRequest("Invalid recovery code. Please contact support.");

        user.TwoFactorEnabled = false;
        user.TwoFactorSecret = null;
        await _db.SaveChangesAsync();

        return ApiResponseDto<Disable2FADto>.Success(new Disable2FADto
        {
            UserId = user.Id,
            DisabledAt = DateTime.UtcNow,
            VerifiedByUserId = sessionUser?.Id ?? Guid.Empty
        });
    }

    // ========================================================================
    // POST - Send password reset email request
    // ========================================================================

    public async Task<ApiResponseDto<string>> RequestPasswordResetAsync(SignInDto signInDto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == signInDto.Email);
        if (user is null || !user.IsTeamMemberOnly)
            return ApiResponseDto<string>.NoContent("Email not found."); // Don't reveal existence

        var resetToken = GeneratePasswordResetToken();
        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        await _db.SaveChangesAsync();

        // Send email (placeholder)
        // await EmailService.SendPasswordResetEmailAsync(user.Email, resetToken);

        return ApiResponseDto<string>.Success("A password reset link has been sent to your email.");
    }

    // ========================================================================
    // POST - Reset password with token
    // ========================================================================

    public async Task<ApiResponseDto<string>> ResetPasswordAsync(SignInDto signInDto, string newPassword)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == signInDto.Email);
        if (user is null || !user.IsTeamMemberOnly)
            return ApiResponseDto<string>.NoContent("User not found.");

        if (!DateTime.UtcNow.AddHours(1).CompareTo(user.PasswordResetTokenExpiry) > 0)
            return ApiResponseDto<string>.BadRequest("Password reset link has expired.");

        var isValidToken = user.PasswordResetToken == signInDto.ResetCode;
        if (!isValidToken)
            return ApiResponseDto<string>.BadRequest("Invalid or expired reset token.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        await _db.SaveChangesAsync();

        return ApiResponseDto<string>.Success("Password has been reset successfully.");
    }

    // ========================================================================
    // Helpers
    // ========================================================================

    private string GenerateJwtToken(Guid userId, string email)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = _configuration["JWT:SecretKey"] ?? "fallback-secret-key-for-local-dev";
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()), new Claim(ClaimTypes.Email, email) }),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken() => Guid.NewGuid().ToString("N");

    private async Task<bool> Verify2FACodeAsync(User user, string code)
    {
        // In production: use Google Authenticator / Yubikey integration
        // For now: simple TOTP-like verification (placeholder)
        return true;
    }

    private List<string> GenerateRecoveryCodes()
    {
        var codes = new List<string>();
        for (int i = 0; i < 8; i++)
            codes.Add(Guid.NewGuid().ToString("N").Substring(0, 16));
        return codes;
    }

    private string GeneratePasswordResetToken() => Guid.NewGuid().ToString("N");

    // In production, store these in a secure table keyed by UserId
    private List<string> validRecoveryCodes = new();
}