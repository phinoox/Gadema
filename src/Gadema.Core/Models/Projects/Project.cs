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
/// Supports polymorphic ownership pattern (User or Team).
/// </summary>
public class Project
{
    /// <summary>
    /// Unique identifier for the project.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Title of the project (e.g., "Fantasy Book Series").
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
    /// Owner type (User = 0, Team = 1).
    /// </summary>
    public int OwnerType { get; set; }

    /// <summary>
    /// FK to User or Team (polymorphic FK pattern).
    /// </summary>
    [Required]
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Navigation property: Soft delete flag.
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Reference navigation properties (Many-to-One relationships)
    /// <summary>
    /// Navigation property: User or Team who owns this project (polymorphic ownership).
    /// Note: This is a weak navigation - use OwnerType to determine actual owner entity type.
    /// </summary>
    [ForeignKey("OwnerId")]
    public virtual User? Owner { get; set; }

    // Collection navigation properties
    /// <summary>
    /// Navigation property: Collection of project tokens for this project.
    /// Enables lazy loading to access all API tokens owned by the project.
    /// Foreign key: ProjectId (matches FK in ProjectToken)
    /// </summary>
    public virtual ICollection<ProjectToken> ProjectTokens { get; set; } = new List<ProjectToken>();

    /// <summary>
    /// Navigation property: Collection of content items for this project.
    /// Enables lazy loading to access all content in the project.
    /// Foreign key: ProjectId (matches FK in ContentItem)
    /// </summary>
    public virtual ICollection<ContentItem> ContentItems { get; set; } = new List<ContentItem>();

    /// <summary>
    /// Navigation property: Collection of sequences for this project.
    /// Enables lazy loading to access all story sequences/chapters in the project.
    /// Foreign key: ProjectId (matches FK in StorySequence)
    /// </summary>
    public virtual ICollection<StorySequence> Sequences { get; set; } = new List<StorySequence>();

    /// <summary>
    /// Navigation property: Collection of tasks for this project.
    /// Enables lazy loading to access all tasks associated with the project.
    /// Foreign key: ProjectId (matches FK in ProjectTask)
    /// </summary>
    public virtual ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();

    /// <summary>
    /// Navigation property: Collection of activity logs for this project.
    /// Enables lazy loading to track all project events.
    /// Foreign key: ProjectId (matches FK in ActivityLog)
    /// </summary>
    public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();

    /// <summary>
    /// Navigation property: Collection of team members for this project.
    /// Enables lazy loading to access all team members associated with the project.
    /// Foreign key: ProjectId (matches FK in TeamMember)
    /// </summary>
    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

    /// <summary>
    /// Navigation property: Collection of tags for this project.
    /// Enables lazy loading to access all tags associated with the project.
    /// Foreign key: ProjectId (matches FK in Tag)
    /// </summary>
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    /// <summary>
    /// Navigation property: Collection of media attachments for this project.
    /// Enables lazy loading to access all media files associated with the project.
    /// Foreign key: ProjectId (matches FK in MediaAttachment)
    /// </summary>
    public virtual ICollection<MediaAttachment> MediaAttachments { get; set; } = new List<MediaAttachment>();

    // Back-reference navigation property for series tracking
    /// <summary>
    /// Navigation property: Parent project in a series (nullable).
    /// Used for multi-part narratives (e.g., "Book 2" of "The Elder Scrolls").
    /// Foreign key: SeriesId matches FK column on child projects.
    /// </summary>
    [Fixture(FixtureHintEnum.Omit)]
    [ForeignKey("SeriesProjectId")]
    public virtual Project? SeriesProject { get; set; }

    /// <summary>
    /// FK to parent project in series (nullable).
    /// Used for back-referencing parent project in a series.
    /// </summary>
    [Fixture(FixtureHintEnum.Omit)]
    public Guid? SeriesProjectId { get; set; }

     /// <summary>
    /// Navigation property: Collection of child projects in this project's series.
    /// Enables lazy loading to access all sequels / parts belonging to this parent project.
    /// Foreign key: SeriesIds (auto-created by EF Core on the many side)
    /// </summary>
    public virtual ICollection<Project> SeriesProjects { get; set; } = new List<Project>();

    /// <summary>
    /// Optional parent series name.
    /// </summary>
    [MaxLength(512)]
    public string? SeriesName { get; set; }
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
    /// FK to User who created the project.
    /// </summary>
    public Guid CreatedByUserId { get; set; }
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

    /// <summary>
    /// URL-friendly slug for series-related queries.
    /// </summary>
    [MaxLength(128), Column("series_id")]
    public string SeriesId { get; set; } = "";

}
