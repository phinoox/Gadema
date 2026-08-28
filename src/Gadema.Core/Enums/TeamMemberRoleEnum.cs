using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Enums;

/// <summary>
/// Team member roles for team collaboration and permission management.
/// </summary>
public enum TeamMemberRoleEnum
{
    /// <summary>
    /// Full access with administrative privileges, can manage all aspects of the project.
    /// </summary>
    [Display(Name = "Administrator")]
    Admin = 0,

    /// <summary>
    /// Editor role with full content editing and task management capabilities.
    /// </summary>
    [Display(Name = "Editor")]
    Editor = 1,

    /// <summary>
    /// Viewer role with read-only access to project content.
    /// </summary>
    [Display(Name = "Viewer")]
    Viewer = 2,
}
