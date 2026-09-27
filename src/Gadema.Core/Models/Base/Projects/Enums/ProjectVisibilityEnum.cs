namespace Gadema.Core.Models.Base.Projects.Enums;

/// <summary>
/// Defines the visibility and access levels for projects within the system.
/// </summary>
public enum ProjectVisibilityEnum
{
    /// <summary>The project is restricted to authorized users or specific team members.</summary>
    [Display(Name = "Private")]
    Private = 0,

    /// <summary>The project is open for discovery and viewing by all community members.</summary>
    [Display(Name = "Public")]
    Public = 1,
}

