using System.ComponentModel.DataAnnotations;
using Gadema.Core.Models.Access.Enums;

namespace Gadema.Core.Models.Access;

/// <summary>
/// Represents a user account within the GaDeMa system.
/// Supports multiple authentication methods including Google OAuth and standard password login.
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class User
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique username used for authentication and identification.
    /// </summary>
    [Required, Display(Name = "Username")]
    public string UserName { get; set; } = "";

    /// <summary>
    /// Gets or sets the unique email address associated with the user account.
    /// </summary>
    [Required, MaxLength(256), EmailAddress, Display(Name = "Email Address")]
    public string Email { get; set; } = "";

    /// <summary>
    /// Gets or sets the user's full name for display purposes.
    /// </summary>
    [MaxLength(4096), Display(Name = "Full Name")]
    public string? DisplayName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the URL to the user's profile image.
    /// </summary>
    [MaxLength(1024)]
    public string? ImageUri { get; set; }


    /// <summary>
    /// Gets or sets the hashed recovery code used for account access recovery.
    /// </summary>
    [MaxLength(2048)]
    public string? RecoveryCodeHash { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the user account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the timestamp of the user's last successful login.
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the account is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the hashed password for standard authentication.
    /// </summary>
    public string? PasswordHash { get; set; }

    /// <summary>
    /// Gets or sets the primary authentication provider used by this user.
    /// </summary>
    public UserAuthProviderEnum Provider { get; set; } = UserAuthProviderEnum.Password;

    /// <summary>
    /// Gets or sets the collection of external identity provider links (e.g., Google OAuth).
    /// </summary>
    public virtual ICollection<UserProviderLink> ProviderLinks { get; set; } = new List<UserProviderLink>();

    /// <summary>
    /// Gets or sets a value indicating whether the user's email address has been verified.
    /// </summary>
    public bool EmailConfirmed { get; set; }
}
