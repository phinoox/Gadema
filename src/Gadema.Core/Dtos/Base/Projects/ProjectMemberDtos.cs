using Gadema.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Projects;

public class AddProjectMemberDto
{
    [Required] public Guid UserId { get; set; }
    [Required] public ProjectMemberRoleEnum Role { get; set; }
}

public class UpdateProjectMemberRoleDto
{
    [Required] public ProjectMemberRoleEnum Role { get; set; }
}

public class ProjectMemberResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public ProjectMemberRoleEnum Role { get; set; }
    public DateTime JoinedAt { get; set; }
}