// =============================================================================
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Projects;

/// <summary>
/// Represents a project in the game development management system.
/// Owned by a single User (CreatedBy).
/// </summary>
[DependencyResolver.ModelDependency(typeof(User))]
public class Project
{
    /// <summary>
    /// Unique identifier for the project.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Title of the project.
    /// </summary>
    [Required, Display(Name = "Project Title")]
    public string Title { get; set; } = "";

    /// <summary>
    /// URL-friendly slug for the project (unique).
    /// </summary>
    [MaxLength(128), Column("slug"), Required, Display(Name = "URL Slug")]
    public string Slug { get; set; } = "";

    /// <summary>
    /// Project-wide description.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; } = null!;

    /// <summary>
    /// Soft delete flag.
    /// </summary>
    public bool IsActive { get; set; } = true;

    // --- Relationships ---

    /// <summary>
    /// FK to the User who created this project.
    /// </summary>
    public Guid OwnerId { get; set; }
    
    /// <summary>
    /// Navigation property: User who created this project.
    /// </summary>
    [ForeignKey("OwnerId")]
    public virtual User Owner { get; set; } = null!;

    /// <summary>
    /// FK to the ProjectSeries this project belongs to.
    /// </summary>
    public Guid? ProjectSeriesId { get; set; }

    /// <summary>
    /// Navigation property: The series this project belongs to.
    /// </summary>
    [ForeignKey("ProjectSeriesId")]
    public virtual ProjectSeries? ProjectSeries { get; set; }

    /// <summary>
    /// Collection of teams with access to this project.
    /// </summary>
    public virtual ICollection<ProjectTeam> ProjectTeams { get; set; } = new List<ProjectTeam>();

    /// <summary>
    /// Collection of project tokens for this project.
    /// </summary>
    public virtual ICollection<ProjectToken> ProjectTokens { get; set; } = new List<ProjectToken>();

    /// <summary>
    /// Collection of content items for this project.
    /// </summary>
    public virtual ICollection<ContentItem> ContentItems { get; set; } = new List<ContentItem>();

    /// <summary>
    /// Collection of sequences for this project.
    /// </summary>
    public virtual ICollection<StorySequence> Sequences { get; set; } = new List<StorySequence>();

    /// <summary>
    /// Collection of tasks for this project.
    /// </summary>
    public virtual ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();

    /// <summary>
    /// Collection of activity logs for this project.
    /// </summary>
    public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();

    // --- Metadata ---

    /// <summary>
    /// Enable user registration flag (feature flag).
    /// </summary>
    public bool EnableUserRegistration { get; set; } = false;

    /// <summary>
    /// Allow manual team invites flag.
    /// </summary>
    public bool AllowManualInvites { get; set; } = true;

    /// <summary>
    /// Default view mode (PrivateWriting or Presentation).
    /// </summary>
    [EnumDataType(typeof(ViewModeEnum)), Required, Display(Name = "Default View Mode")]
    public ViewModeEnum ViewMode { get; set; } = ViewModeEnum.PrivateWriting;

    /// <summary>
    /// Project creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ProjectVisibilityEnum Visibility { get; set; }

    /// <summary>
    /// Last modified timestamp.
    /// </summary>
    [Column("last_modified_at")]
    public DateTime? LastModifiedAt { get; set; } = null!;

    /// <summary>
    /// Status of the project (Draft, InProgress, Published).
    /// </summary>
    [EnumDataType(typeof(ProjectStatusEnum)), Required, Display(Name = "Status")]
    public ProjectStatusEnum Status { get; set; } = ProjectStatusEnum.Draft;

    public virtual ICollection<ProjectTagRelation> ProjectTags { get; set; } = new List<ProjectTagRelation>();
}