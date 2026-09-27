using Gadema.Core.Enums;
using Gadema.Core.Models.Access.Enums;

namespace Gadema.Core.Models.Access;

/// <summary>
/// Represents a link between a user account and an external authentication provider.
/// This provides an audit trail of which providers are associated with a specific user.
/// </summary>
[ModelDependency(typeof(User))]
public class UserProviderLink
{
    /// <summary>
    /// Gets or sets the unique identifier of the associated user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the associated user entity.
    /// </summary>
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the authentication provider used for this link (e.g., Google).
    /// </summary>
    public UserAuthProviderEnum Provider { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this provider was linked to the user account.
    /// </summary>
    public DateTime LinkedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the unique identifier provided by the external service (e.g., Google's 'sub' claim).
    /// </summary>
    public string? ExternalSubjectId { get; set; } // e.g. Google "sub"
}