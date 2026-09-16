namespace Gadema.Core.Models.Access.Enums;

public enum UserAuthProviderEnum
{
    Password = 0,   // Email/password (PBKDF2) — may additionally have TOTP 2FA
    Google = 1      // Google OAuth (ID-token `sub`)
}