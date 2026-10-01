namespace Gadema.Core.Models.Access;

using System.ComponentModel.DataAnnotations;
using Gadema.Core.Models.Access;

/// <summary>
/// An extension of the User identity, adding external provider data.
/// Following Law I: This is a 1:1 extension, so Id == UserId.
/// </summary>
public class UserProviderLink
{
    [Key]
    public Guid Id { get; set; } // Same as UserId

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
