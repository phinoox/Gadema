// =============================================================================
using GameDev.Core.Enums;
using GameDev.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models.Projects;

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
    [ForeignKey(nameof(Owner))]
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Navigation property: Soft delete flag.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property: Project that owns this project (User/Team polymorphic ownership).
    /// Note: This is a weak navigation - use OwnerType to determine actual owner entity type.
    /// </summary>
    public virtual Project? Owner { get; set; }
    /// <summary>
    /// Optional parent project ID for series tracking.
    /// </summary>
    [MaxLength(128)]
    public string? SeriesId { get; set; }
    /// <summary>
    /// Navigation property: Parent project for series tracking (Restrict to maintain history).
    /// </summary>
    public virtual Project? SeriesProject { get; set; }
    /// <summary>
    /// Project visibility (Private = 0, Public = 1).
    /// </summary>
    [EnumDataType(typeof(ProjectVisibilityEnum)), Required, Display(Name = "Visibility")]
    public ProjectVisibilityEnum Visibility { get; set; } = ProjectVisibilityEnum.Private;


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
    /// Navigation property: Collection of activity logs for this project.
    /// Enables lazy loading to track all project events.
    /// Foreign key: ProjectId (matches FK in ActivityLog)
    /// </summary>
    public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    /// <summary>
    /// Project status (Draft = 0, InProgress = 1, Published = 2).
    /// </summary>
    public int Status { get; set; } = 0;
    
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
    
    /// <summary>
    /// Last modified timestamp.
    /// </summary>
    [Column("last_modified_at")]
    public DateTime? LastModifiedAt { get; set; } = null!;

}

