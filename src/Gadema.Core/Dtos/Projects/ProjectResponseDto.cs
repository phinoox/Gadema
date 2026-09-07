// =============================================================================
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
    /// Project-wide description.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    /// <summary>
    /// ID of the user who created this project.
    /// </summary>
    [Display(Name = "User ID")]
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Series ID (optional).
    /// </summary>
    public Guid? ProjectSeriesId { get; set; }
    
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
    public ViewModeEnum ViewMode { get; set; }
    
    /// <summary>
    /// Project creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Last modified timestamp.
    /// </summary>
    [Display(Name = "Last Modified At")]
    public DateTime? LastModifiedAt { get; set; }
    
    /// <summary>
    /// Primary format of the project (Book, Manga, Game, Hybrid).
    /// </summary>
    [Display(Name = "Primary Format")]
    public PrimaryFormatEnum PrimaryFormat { get; set; }
    
    /// <summary>
    /// Genre of the project.
    /// </summary>
    [Display(Name = "Genre")]
    public string? Genre { get; set; }
    
    /// <summary>
    /// Central theme of the project.
    /// </summary>
    [Display(Name = "Theme")]
    public string? Theme { get; set; }
    
    /// <summary>
    /// Tone of the project.
    /// </summary>
    [Display(Name = "Tone")]
    public ToneEnum Tone { get; set; }
    
    /// <summary>
    /// Target audience for the project.
    /// </summary>
    [Display(Name = "Audience")]
    public AudienceEnum Audience { get; set; }
}