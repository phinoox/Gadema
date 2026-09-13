// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Authentication;

/// <summary>
/// Service for 2FA (Two-Factor Authentication) operations.
/// Handles QR code generation, TOTP verification, and recovery codes.
/// </summary>
public class TwoFactorAuthService : CoreService
{
    public TwoFactorAuthService(GameDbContext db, ILogger<TwoFactorAuthService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET /api/v1/auth/tfa/qr/{userId} - Get QR code for authenticator app setup
    // ========================================================================

    public async Task<ApiResponseDto<QrCodeResponse>> GetQrCodeAsync(Guid userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null || !user.IsTeamMemberOnly)
            return ApiResponseDto<QrCodeResponse>.Forbidden("User not found or not eligible for 2FA.");

        var sessionUser = _userContext.CurrentUser;
        if (sessionUser == null || user.Id != sessionUser.Id && !_db.Users.Any(u => u.Id == userId && BCrypt.Net.BCrypt.Verify(sessionUser.PasswordHash, u.PasswordHash)))
            return ApiResponseDto<QrCodeResponse>.Unauthorized("Invalid credentials.");

        var qrCodeData = $"otpauth://totp/Gadema:user:{user.Email}?secret={user.TwoFactorSecret}&issuer=Gadema";
        var qrUrl = $"https://chart.googleapis.com/chart?chs=200x200&cht=qr&chl={Uri.EscapeDataString(qrCodeData)}";

        return ApiResponseDto<QrCodeResponse>.Success(new QrCodeResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Secret = user.TwoFactorSecret!,
            QrCodeUrl = qrUrl,
            Issuer = "Gadema",
            AccountName = user.Email
        });
    }

    // ========================================================================
    // POST /api/v1/auth/tfa/verify - Verify TOTP code from authenticator app
    // ========================================================================

    public async Task<ApiResponseDto<bool>> Verify2FACodeAsync(Guid userId, string totpCode)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null || !user.IsTeamMemberOnly)
            return ApiResponseDto<bool>.Forbidden("User not found or not eligible for 2FA.");

        // Verify TOTP code (placeholder - in production use a proper TOTP library like Google.Authenticator)
        var isValid = await VerifyTotpCode(user.TwoFactorSecret, totpCode);

        if (!isValid)
            return ApiResponseDto<bool>.BadRequest("Invalid 2FA code. Please check your authenticator app.");

        // Mark the last verified time (optional: for rate limiting)
        user.Last2FAVerifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponseDto<bool>.Success(true);
    }

    // ========================================================================
    // GET /api/v1/auth/tfa/recovery-codes - Get recovery codes (admin only)
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<string>>> GetRecoveryCodesAsync(Guid userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null || !user.IsTeamMemberOnly)
            return ApiResponseDto<IEnumerable<string>>.Forbidden("User not found or not eligible for 2FA.");

        var sessionUser = _userContext.CurrentUser;
        if (sessionUser?.Id != user.Id)
            return ApiResponseDto<IEnumerable<string>>.Forbidden("You can only retrieve your own recovery codes.");

        var codes = GenerateRecoveryCodes(user.Id);

        return ApiResponseDto<IEnumerable<string>>.Success(codes);
    }

    // ========================================================================
    // HELPER: Verify TOTP code using Google Authenticator algorithm (simplified)
    // ========================================================================

    private async Task<bool> VerifyTotpCode(string secret, string code)
    {
        if (!ulong.TryParse(secret, out var secretNum))
            return false;

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var steam = SteamTotp.GenerateSteam((int)(timestamp % 30), secretNum);
        var hash = SHA1.HashData(Encoding.UTF8.GetBytes(steam + code));
        var expectedCode = Convert.ToUInt64(hash, 16).ToString("D8");

        // Allow ±30 seconds window (2 tokens)
        for (int i = -1; i <= 1; i++)
        {
            var adjustedTimestamp = timestamp + (i * 30);
            var adjustedSteam = SteamTotp.GenerateSteam((int)(adjustedTimestamp % 30), secretNum);
            var adjustedHash = SHA1.HashData(Encoding.UTF8.GetBytes(adjustedSteam + code));
            var adjustedCode = Convert.ToUInt64(adjustedHash, 16).ToString("D8");

            if (code == adjustedCode)
                return true;
        }

        return false;
    }

    // ========================================================================
    // HELPER: Generate new recovery codes
    // ========================================================================

    private List<string> GenerateRecoveryCodes(Guid userId)
    {
        var codes = new List<string>();
        for (int i = 0; i < 8; i++)
            codes.Add(Guid.NewGuid().ToString("N").Substring(0, 16));

        return codes;
    }
}