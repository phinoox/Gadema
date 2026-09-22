using Gadema.Core.Models.Base.Projects;

namespace Gadema.Core.Models.Access;

public enum ProjectMemberRoleEnum
{
    Owner = 0,
    Admin = 1,
    Editor = 2,
    Reviewer = 3,
    Viewer = 4
}

[ModelDependency(typeof(Project),typeof(User))]
public class ProjectMember
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public ProjectMemberRoleEnum Role { get; set; }
    public DateTime JoinedAt { get; set; }
    // Navigation
    [Required]
    public virtual Project Project { get; set; } = null!;

    [Required]
    public virtual User User { get; set; } = null!;
}