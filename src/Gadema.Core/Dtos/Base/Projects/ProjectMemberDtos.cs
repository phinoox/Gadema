namespace Gadema.Core.Dtos.Base.Projects;

using Gadema.Core.Models.Access;


/// <summary>
/// Data transfer object for adding a new member to a project.
/// </summary>
public class AddProjectMemberDto
{
    /// <summary>
    /// The unique identifier of the user being added.
    /// </summary>
    [Required] public Guid UserId { get; set; }

    /// <summary>
    /// The initial role assigned to this member in the project.
    /// </summary>
    [Required] public ProjectMemberRoleEnum Role { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing project member's role.
/// </summary>
public class UpdateProjectMemberRoleDto
{
    /// <summary>
    /// The new role to be assigned to the member.
    /// </summary>
    [Required] public ProjectMemberRoleEnum Role { get; set; }
}

/// <summary>
/// Represents a project membership record for response purposes.
/// </summary>
public class ProjectMemberResponseDto
{
    /// <summary>
    /// The unique identifier of the membership record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The unique identifier of the user in the project.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// The full name of the member.
    /// </summary>
    public string UserName { get; set; } = "";

    /// <summary>
    /// The email address of the member.
    /// </summary>
    public string Email { get; set; } = "";

    /// <summary>
    /// The current role held by this member in the project.
    /// </summary>
    public ProjectMemberRoleEnum Role { get; set; }

    /// <summary>
    /// The timestamp when the user joined the project.
    /// </summary>
    public DateTime JoinedAt { get; set; }
}