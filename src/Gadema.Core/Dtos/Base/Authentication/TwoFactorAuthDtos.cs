using Gadema.Core.Dtos;
using Gadema.Core.Enums;

namespace Gadema.Core.Dtos.Authentication;

public class Enable2FAResponseDto
{
    public string Base32Secret { get; set; } = "";
    public string OtpAuthUri { get; set; } = ""; // otpauth:// URI for QR code
    public int ConfirmTokenMinutes { get; set; }
    public string ConfirmToken { get; set; }
}

/// <summary>Step 2 of 2FA sign-in: pending token + TOTP code or recovery code.</summary>
public class SignIn2FADto
{
    public string PendingToken { get; set; } = "";
    public string Code { get; set; } = "";               // 6-digit TOTP
    public string? RecoveryCode { get; set; }            // alternative to Code
}

/// <summary>Confirm 2FA setup with a valid TOTP code from the authenticator.</summary>
public class Confirm2FADto
{
    public string SetupToken { get; set; } = "";         // short-lived token from Enable
    public string Code { get; set; } = "";               // first valid TOTP code
}

/// <summary>Link another provider (e.g. Google) to an existing account.</summary>
public class LinkProviderDto
{
    public Guid ExistingUserId { get; set; }
    public UserAuthProviderEnum Provider { get; set; }
    public string? ExternalSubjectId { get; set; }
    public string? Password { get; set; }
}