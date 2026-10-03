namespace Gadema.Core.Dtos.Access;

using System.ComponentModel.DataAnnotations;
using Gadema.Core.Dtos.Base.Infrastructure;

/// <summary>
/// Represents the extended profile information for a user.
/// </summary>
public class UserProfileResponseDto
{
    /// <summary>
    /// The unique identifier of the user (shared with UserResponse).
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// The title/display name of the user.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The short description or bio of the user.
    /// </summary>
    public string? ShortDesc { get; set; }

    /// <summary>
    /// The URL-friendly slug for the profile.
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// URL to the user's avatar image.
    /// </summary>
    public string? AvatarUrl { get; set; }
}

/// <summary>
/// Data transfer object for updating a user's profile via the sync pattern.
/// </summary>
public class UserProfileUpdateDto : BaseMetaInfoUpdateData
{
    // Inherits Title, Slug, ShortDesc, IsPublic (from BaseMetaInfoUpdateData)
    // We can add AvatarUrl here as it is specific to user profiles and not part of the base meta info.
    
    /// <summary>
    /// URL to the user's avatar image.
    /// </summary>
    public string? AvatarUrl { get; set; }
}
