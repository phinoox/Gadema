using System.Security.Cryptography;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Authentication;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Authentication;

public class TwoFactorAuthService
{
    private readonly GameDbContext _db;
    private readonly JwtTokenService _jwt;

    public TwoFactorAuthService(GameDbContext db, JwtTokenService jwt)
    {
        _db  = db;
        _jwt = jwt;
    }

    // ── Enable ──────────────────────────────────────────────────────────
    // Generates a new secret, returns it (Base32) + otpauth URI.
    // The secret is NOT activated until Confirm succeeds.
    public ApiResponseDto<Enable2FAResponseDto> Enable(Guid userId)
    {
        var user = _db.Users.Find(userId);
        if (user == null)
            return ApiResponseDto<Enable2FAResponseDto>.NotFound("User not found.");
        if (user.TwoFactorEnabled)
            return ApiResponseDto<Enable2FAResponseDto>.Conflict("2FA is already enabled.");

        var secret = TotpService.GenerateSecret();
        // Store a *draft* secret (flag TwoFactorEnabled = false until confirmed)
        user.TwoFactorSecret = secret;
        user.TwoFactorEnabled = false;
        _db.SaveChanges();

        var otpUri = $"otpauth://totp/GaDeMa:{user.Email}" +
                     $"?secret={secret}&issuer=GaDeMa&algorithm=SHA1&digits=6&period=30";
        var setupToken = _jwt.IssuePendingToken(userId, "2fa_setup");

        return ApiResponseDto<Enable2FAResponseDto>.Success(new Enable2FAResponseDto
        {
            Base32Secret     = secret,
            OtpAuthUri       = otpUri,
            ConfirmToken     = setupToken,
            ConfirmTokenMinutes = 8
        });
    }

    // ── Confirm ─────────────────────────────────────────────────────────
    // Validates the first TOTP code from the authenticator, activates 2FA.
    public ApiResponseDto<AuthResponse> Confirm(Guid userId, Confirm2FADto dto)
    {
        var user = _db.Users.Find(userId);
        if (user == null)
            return ApiResponseDto<AuthResponse>.NotFound("User not found.");

        var (valid, tokenUserId, purpose) = _jwt.Validate(dto.SetupToken);
        if (!valid || tokenUserId != userId || purpose != "2fa_setup")
            return ApiResponseDto<AuthResponse>.BadRequest("Invalid or expired setup token.");

        if (string.IsNullOrWhiteSpace(user.TwoFactorSecret)
            || !TotpService.Validate(user.TwoFactorSecret, dto.Code))
            return ApiResponseDto<AuthResponse>.Unauthorized("Invalid authenticator code.");

        user.TwoFactorEnabled = true;
        user.RecoveryCodeHash = HashRecoveryCode(GenerateRecoveryCode()); // seed first code
        _db.SaveChanges();

        return Success(user);
    }

    // ── Disable ─────────────────────────────────────────────────────────
    public ApiResponseDto<AuthResponse> Disable(Guid userId, string code)
    {
        var user = _db.Users.Find(userId);
        if (user == null || !user.TwoFactorEnabled)
            return ApiResponseDto<AuthResponse>.BadRequest("2FA is not enabled.");

        if (!TotpService.Validate(user.TwoFactorSecret!, code))
            return ApiResponseDto<AuthResponse>.Unauthorized("Invalid authenticator code.");

        user.TwoFactorEnabled = false;
        user.TwoFactorSecret  = null;
        user.RecoveryCodeHash = null;
        _db.SaveChanges();

        return Success(user);
    }

    // ── Get / regenerate recovery codes ─────────────────────────────────
    // Returns ONE single-use code (client displays it, stores it locally).
    // A new code is generated and its hash stored on each call.
    public ApiResponseDto<RecoveryCodesResponseDto> GetRecoveryCodes(Guid userId)
    {
        var user = _db.Users.Find(userId);
        if (user == null || !user.TwoFactorEnabled)
            return ApiResponseDto<RecoveryCodesResponseDto>.BadRequest("2FA is not enabled.");

        var code = GenerateRecoveryCode();
        user.RecoveryCodeHash = HashRecoveryCode(code);
        _db.SaveChanges();

        return ApiResponseDto<RecoveryCodesResponseDto>.Success(new RecoveryCodesResponseDto
        {
            Codes   = new List<string>(){code},
            Message = "Single-use recovery code. Store it securely — it cannot be retrieved again."
        });
    }

    // ── Step 2 of sign-in ───────────────────────────────────────────────
    // Accepts either a TOTP code OR a recovery code.
    public ApiResponseDto<AuthResponse> CompleteSignIn(SignIn2FADto dto)
    {
        var (valid, userId, purpose) = _jwt.Validate(dto.PendingToken);
        if (!valid || purpose != "2fa_signin" || userId == null)
            return ApiResponseDto<AuthResponse>.BadRequest("Invalid or expired pending token.");

        var user = _db.Users.Find(userId.Value);
        if (user == null || !user.TwoFactorEnabled)
            return ApiResponseDto<AuthResponse>.Unauthorized("2FA is not enabled for this account.");

        bool ok;
        if (!string.IsNullOrWhiteSpace(dto.RecoveryCode))
        {
            ok = CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(HashRecoveryCode(dto.RecoveryCode)),
                Convert.FromBase64String(user.RecoveryCodeHash!));
            if (ok)
            {
                // Invalidate the used code
                user.RecoveryCodeHash = null;
            }
        }
        else
        {
            ok = TotpService.Validate(user.TwoFactorSecret!, dto.Code);
        }

        if (!ok)
            return ApiResponseDto<AuthResponse>.Unauthorized("Invalid 2FA code.");

        user.LastLogin = DateTime.UtcNow;
        _db.SaveChanges();

        return Success(user);
    }

    // ── Private helpers ─────────────────────────────────────────────────

    private ApiResponseDto<AuthResponse> Success(User user)
    {
        // Reuse JwtTokenService — auth_method reflects the user's primary provider
        var method = user.Provider == UserAuthProviderEnum.Google ? "google" : "password";
        return ApiResponseDto<AuthResponse>.Success(new AuthResponse
        {
            AccessToken       = _jwt.IssueAccessToken(user.Id, user.Email, user.FullName, method),
            TokenType         = "Bearer",
            ExpiresInSeconds  = 3600,
            User              = new UserResponse
            {
                Id              = user.Id.ToString(),
                Name            = user.FullName ?? "",
                Email           = user.Email,
                GoogleSubjectId = user.GoogleSubjectId
            },
            Message = "Authenticated."
        });
    }

    private static string GenerateRecoveryCode()
    {
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(8);
        return Convert.ToHexString(bytes).Insert(4, "-").Insert(9, "-"); // XXXX-XXXX-XXXX
    }

    private static string HashRecoveryCode(string code)
    {
        // SHA-256, no salt needed (code is already random + single-use)
        var hash = System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(code));
        return Convert.ToBase64String(hash);
    }
}