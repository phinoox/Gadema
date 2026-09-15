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
    public string? DisplayName { get; set; } = null!;

    /// <summary>
    /// URL to user profile image.
    /// </summary>
    [MaxLength(1024)]
    public string? ImageUri { get; set; }


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

    public string? PasswordHash { get; set; }

    // add to existing User class:
    public UserAuthProviderEnum Provider { get; set; } = UserAuthProviderEnum.Password;
    public virtual ICollection<UserProviderLink> ProviderLinks { get; set; } = new List<UserProviderLink>();
    public bool EmailConfirmed { get; set; }
    // (RecoveryCodeHash, TwoFactorSecret, PasswordHash already exist — keep them)
}