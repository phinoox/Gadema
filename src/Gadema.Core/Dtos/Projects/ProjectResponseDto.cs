// =============================================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Enums;
// =============================================================================

namespace Gadema.Core.Dtos.Projects;

/// <summary>
/// DTO for project list/response.
/// </summary>
public class ProjectResponseDto
{
    /// <summary>
    /// Unique identifier for the project.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Project title.
    /// </summary>
    [Display(Name = "Project Title")]
    public string Title { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the project.
    /// </summary>
    [Display(Name = "URL Slug")]
    public string Slug { get; set; } = "";
    
    /// <summary>
    /// Owner type: 0=User, 1=Team.
    /// </summary>
    public int OwnerType { get; set; }
    
    /// <summary>
    /// ID of the owner (user or team).
    /// </summary>
    public Guid OwnerId { get; set; }
    
    /// <summary>
    /// Series ID (optional).
    /// </summary>
    public Guid? SeriesId { get; set; } = null!;
    
    /// <summary>
    /// Visibility: 1=Private, 2=Public.
    /// </summary>
    [Display(Name = "Visibility")]
    public ProjectVisibilityEnum Visibility { get; set; }
    
    /// <summary>
    /// Status: 0=Draft, 1=InProgress, 2=Published, 3=Archived.
    /// </summary>
    [Display(Name = "Status")]
    public ProjectStatusEnum Status { get; set; }
    
    /// <summary>
    /// Enable user registration flag.
    /// </summary>
    public bool EnableUserRegistration { get; set; }
    
    /// <summary>
    /// Allow manual invites flag.
    /// </summary>
    public bool AllowManualInvites { get; set; }
    
    /// <summary>
    /// View mode: PrivateWriting or Presentation.
    /// </summary>
    [Display(Name = "View Mode")]
    public string ViewMode { get; set; } = "";
    
    /// <summary>
    /// Project creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}