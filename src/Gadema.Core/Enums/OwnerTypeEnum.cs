using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Enums;

/// <summary>
/// Specifies the type of entity that owns a project in the polymorphic ownership model.
/// </summary>
public enum OwnerTypeEnum
{
    /// <summary>
    /// The project is owned by an individual user.
    /// </summary>
    [Display(Name = "User")]
    User = 0,

    /// <summary>
    /// The project is owned by a team for collaborative purposes.
    /// </summary>
    [Display(Name = "Team")]
    Team = 1,
}

