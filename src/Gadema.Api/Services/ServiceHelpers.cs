// =============================================================================
using Gadema.Core.Dtos;
// Gadema.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Gadema.Data.Database;
using Microsoft.Extensions.Logging;

namespace Gadema.Api.Services;

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

