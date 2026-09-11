// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

using System.ComponentModel.DataAnnotations;
using Gadema.Core.Enums;


namespace Gadema.Core.Models;

/// <summary>
/// Represents a user account in the GaDeMa system.
/// Supports Google OAuth, password login, and two-factor authentication.
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class User
{
    /// <summary>
    /// Unique identifier for the user.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Username for login (unique).
    /// </summary>
    [Required, Display(Name = "Username")]
    public string UserName { get; set; } = "";

    /// <summary>
    /// Email address for account recovery and notifications (unique).
    /// </summary>
    [Required, MaxLength(256), EmailAddress, Display(Name = "Email Address")]
    public string Email { get; set; } = "";

    /// <summary>
    /// Full name of the user.
    /// </summary>
    [MaxLength(4096), Display(Name = "Full Name")]
    public string? FullName { get; set; } = null!;

    /// <summary>
    /// URL to user profile image.
    /// </summary>
    [MaxLength(1024)]
    public string? ImageUri { get; set; }

    /// <summary>
    /// Google OAuth subject ID for linked accounts.
    /// </summary>
    [MaxLength(512)]
    public string? GoogleSubjectId { get; set; }

    /// <summary>
    /// Indicates if two-factor authentication is enabled.
    /// </summary>
    public bool TwoFactorEnabled { get; set; } = false;

    /// <summary>
    /// Hashed recovery code for account access.
    /// </summary>
    [MaxLength(2048)]
    public string? RecoveryCodeHash { get; set; }

    /// <summary>
    /// Account creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last login timestamp.
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// Indicates if the account is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Collection of team memberships for this user.
    /// Enables lazy loading to access all teams a user belongs to.
    /// Foreign key: UserId (matches FK in TeamMember)
    /// </summary>
    public virtual ICollection<TeamMember> TeamMemberships { get; set; } = new List<TeamMember>();

    /// <summary>
    /// Navigation property: Collection of teams this user owns/created.
    /// Enables lazy loading to access all teams owned by the user.
    /// Foreign key: CreatedByUserId (matches FK in Team)
    /// </summary>
    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
    public string? PasswordHash { get; set; }
    public string? TwoFactorSecret { get; set; }

    // add to existing User class:
    public UserAuthProviderEnum Provider { get; set; } = UserAuthProviderEnum.Password;
    public virtual ICollection<UserProviderLink> ProviderLinks { get; set; } = new List<UserProviderLink>();
    // (RecoveryCodeHash, TwoFactorSecret, PasswordHash already exist — keep them)
}