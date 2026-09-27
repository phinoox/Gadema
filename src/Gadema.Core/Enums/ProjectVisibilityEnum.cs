using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Enums;

/// <summary>
/// Defines the visibility settings for a project to control access.
/// </summary>
public enum ProjectVisibilityEnum
{
    /// <summary>
    /// The project is private and only accessible to authorized users or specific roles.
    /// </summary>
    [Display(Name = "Private")]
    Private = 0,

    /// <summary>
    /// The project is publicly visible and accessible to all users.
    /// </summary>
    [Display(Name = "Public")]
    Public = 1,
}

