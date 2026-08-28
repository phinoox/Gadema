using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Enums;

/// <summary>
/// Project visibility options for access control.
/// </summary>
public enum ProjectVisibilityEnum
{
    /// <summary>
    /// Project is private and only accessible to authenticated users or specific roles.
    /// </summary>
    [Display(Name = "Private")]
    Private = 0,

    /// <summary>
    /// Project is publicly accessible to all users.
    /// </summary>
    [Display(Name = "Public")]
    Public = 1,
}
