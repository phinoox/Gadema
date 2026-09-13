using System.ComponentModel.DataAnnotations;
using Gadema.Core.Models;
using Gadema.Core.Models.Projects;

public enum ProjectMemberRoleEnum
{
    Owner = 0,
    Admin = 1,
    Editor = 2,
    Reviewer = 3,
    Viewer = 4
}

public class ProjectMember
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public ProjectMemberRoleEnum Role { get; set; }
    public DateTime JoinedAt { get; set; }
    // Navigation
    [Required]
    public virtual Project Project { get; set; }

    [Required]
    public virtual User User { get; set; }
}