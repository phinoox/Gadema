namespace Gadema.Core.Dtos.Access;

/// <summary>
/// Represents the identity information of a user returned after successful authentication.
/// </summary>
public class UserResponse
{
    /// <summary>
    /// The unique identifier for the user.
    /// </summary>
    public string Id { get; set; } = "";

    /// <summary>
    /// The full name of the user.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// The primary email address associated with the account.
    /// </summary>
    public string Email { get; set; } = "";
  
}

/// <summary>
/// A unified response containing security tokens and user context for all authentication methods.
/// </summary>
public class AuthResponse
{
// ... existing code ...
    /// <summary>
    /// The JWT bearer token or session identifier used for authenticated requests.
    /// </summary>
    public string AccessToken { get; set; }    // JWT token (Bearer) or session ID

    /// <summary>
    /// The type of authentication mechanism being used (e.g., "Bearer", "Session").
    /// </summary>
    public string TokenType { get; set; }     // "Bearer" | "Session" | "RefreshToken"

    /// <summary>
    /// The time-to-live for the token in seconds.
    /// </summary>
    public int ExpiresInSeconds { get; set; }  // TTL for the returned token

    /// <summary>
    /// An optional human-readable message providing context on success or failure.
    /// </summary>
    public string? Message { get; set; }       // Optional human-readable message on success/failure
                                        // add to existing AuthResponse:
   
    /// <summary>
    /// The user identity information associated with this authentication session.
    /// </summary>
    public UserResponse User {get;set;}   
}

