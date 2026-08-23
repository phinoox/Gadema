// =============================================================================
using GameDev.Core.Dtos;
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GameDev.Data;
using Microsoft.Extensions.Logging;

namespace GameDev.Api.Services;

/// <summary>
/// Helper class for common operations.
/// </summary>
public static class UserHelper
{
    /// <summary>
    /// Get user ID from JWT token (placeholder implementation).
    /// </summary>
    public static Guid GetUserId()
    {
        // In production, extract from JWT claims or session
        return Guid.NewGuid(); // Placeholder - should get from authentication context
    }
}

/// <summary>
/// Helper class for generating slugs.
/// </summary>
public static class SlugHelper
{
    /// <summary>
    /// Generate URL-friendly slug from title.
    /// </summary>
    public static string GenerateSlug(string title)
    {
        return WebUtility.UrlEncode(
            System.Web.HttpUtility.HtmlDecode(System.Web.HttpUtility.UrlEncode(title)).ToLowerInvariant()
                .Replace(" ", "-").Replace("&", "and")
                .Replace("/", "-").Replace("\\", "-")
                .Replace("'", "").Replace("\"", "")
                .Replace("<", "").Replace(">", "")
                .Replace("[", "").Replace("]", "")
                .TrimEnd("-").ToString());
    }
}

/// <summary>
/// Configuration constants.
/// </summary>
public static class FileConfigurationConstants
{
    /// <summary>
    /// Maximum file size for uploads (100MB).
    /// </summary>
    public const int MaxFileSize = 100 * 1024 * 1024;

    /// <summary>
    /// Allowed MIME types.
    /// </summary>
    public static readonly string[] AllowedMimeTypes = 
    {
        "image/png",
        "image/jpeg",
        "image/gif",
        "application/pdf",
        "text/plain",
        "application/json"
    };
}

/// <summary>
/// Response DTO for authentication.
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Access token (JWT).
    /// </summary>
    public string AccessToken { get; set; } = "";

    /// <summary>
    /// Token type.
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Expiration in seconds.
    /// </summary>
    public int ExpiresInSeconds { get; set; } = 3600;

    /// <summary>
    /// Refresh token.
    /// </summary>
    public string? RefreshToken { get; set; } = null!;

    /// <summary>
    /// User info.
    /// </summary>
    public UserResponse? User { get; set; } = null!;
}

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