using System;

namespace Gadema.Core.Dtos.Authentication;

/// <summary>
/// User response DTO.
/// </summary>
public class UserResponse
{
    /// <summary>
    /// User ID.
    /// </summary>
    public string Id { get; set; } = "";

    /// <summary>
    /// Full name.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Email.
    /// </summary>
    public string Email { get; set; } = "";

    /// <summary>
    /// Google Subject ID.
    /// </summary>
    public string? GoogleSubjectId { get; set; } = null!;
}

/// <summary>
/// Unified response shape for ALL authentication methods.
/// The client only needs to understand THIS type, not the request type.
/// </summary>
public class AuthResponse
{
    //public Guid UserId { get; set; }
    //public string Email { get; set; }
    //public string Name { get; set; }
    public string AccessToken { get; set; }    // JWT token (Bearer) or session ID
    public string TokenType { get; set; }     // "Bearer" | "Session" | "RefreshToken"
    public int ExpiresInSeconds { get; set; }  // TTL for the returned token
    public string? Message { get; set; }       // Optional human-readable message on success/failure
                                        // add to existing AuthResponse:
    public bool TwoFactorRequired { get; set; }
    public string? TwoFactorToken { get; set; } // short-lived pending token, sent to 2FA signin
    
    public UserResponse User {get;set;}   
}

