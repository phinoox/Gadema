using System.ComponentModel.DataAnnotations.Schema;
using Gadema.Core.DependencyResolver;
using Gadema.Core.Enums;

namespace Gadema.Core.Models;

/// <summary>
/// Audit trail: which authentication providers are linked to a user account.
/// </summary>
[ModelDependency(typeof(User))]
public class UserProviderLink
{
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    public UserAuthProviderEnum Provider { get; set; }
    public DateTime LinkedAtUtc { get; set; } = DateTime.UtcNow;
    public string? ExternalSubjectId { get; set; } // e.g. Google "sub"
}