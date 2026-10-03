using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Models.Access.Enums;
using Gadema.Core.Models.Base.MetaInfo;

namespace Gadema.Core.Models.Access;

/// <summary>
/// Represents a user account within the GaDeMa system.
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class User
{
    /// <summary>
    /// The unique identifier for the user.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The unique username used for authentication and identification.
    /// </summary>
    [Required, Display(Name = "Username")]
    public string UserName { get; set; } = "";

    /// <summary>
    /// The unique email address associated with the user account.
    /// </summary>
    [Required, MaxLength(256), EmailAddress, Display(Name = "Email Address")]
    public string Email { get; set; } = "";

    /// <summary>
    /// The metadata attached to the user identity (Title, ShortDesc, etc.).
    /// </summary>
    public virtual UserMetaInfo MetaInfo { get; set; } = null!;

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
    /// Gets or sets a value indicating whether the user's email address has been verified.
    /// </summary>
    public bool EmailConfirmed { get; set; }
}
