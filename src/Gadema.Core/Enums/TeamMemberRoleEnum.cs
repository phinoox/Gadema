namespace Gadema.Core.Enums;

/// <summary>
/// Specifies the roles and associated permissions for members within a project team.
/// </summary>
public enum TeamMemberRoleEnum
{
    /// <summary>
    /// Full access with administrative privileges, including the ability to manage all aspects of the project.
    /// </summary>
    [Display(Name = "Administrator")]
    Admin = 0,

    /// <summary>
    /// Editor role with full content editing and task management capabilities, but without full administration rights.
    /// </summary>
    [Display(Name = "Editor")]
    Editor = 1,

    /// <summary>
    /// Viewer role providing read-only access to project content for monitoring or collaboration purposes.
    /// </summary>
    [Display(Name = "Viewer")]
    Viewer = 2,
}

