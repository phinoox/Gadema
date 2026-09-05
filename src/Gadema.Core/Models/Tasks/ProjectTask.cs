// =============================================================================
using Gadema.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Represents a task in the project workflow.
/// Flat structure for ADHD-friendly task management (no hierarchical epics/stories).
/// </summary>
[DependencyResolver.ModelDependency(typeof(Project), typeof(ContentItem))]
public class ProjectTask
{
    /// <summary>
    /// Unique identifier for the task.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// ID of the project this task belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    // Navigation property: Project (Many-to-One)
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }

    /// <summary>
    /// Optional FK to ContentItem (nullable).
    /// </summary>
    public Guid? ContentItemId { get; set; }  // Nullable FK to ContentItem

    // Navigation property: ContentItem (optional)
    [ForeignKey("ContentItemId")]
    public virtual ContentItem? ContentItem { get; set; }

    /// <summary>
    /// Task title.
    /// </summary>
    [MaxLength(256), Required]
    public string TaskTitle { get; set; } = "";

    /// <summary>
    /// Task description.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }

    /// <summary>
    /// Status: 0=Backlog, 1=InProgress, 2=Review, 3=Done.
    /// </summary>
    public int Status { get; set; }  // Enum: Backlog, InProgress, Review, Done

    /// <summary>
    /// Priority: 0=High, 1=Medium, 2=Low.
    /// </summary>
    public int Priority { get; set; }  // Enum: High, Medium, Low

    /// <summary>
    /// Difficulty: 0=Easy, 1=Medium, 2=Hard (ADHD-friendly).
    /// </summary>
    public int Difficulty { get; set; }  // Enum: Easy, Medium, Hard

    /// <summary>
    /// Estimated time in minutes.
    /// </summary>
    [MaxLength(512)]
    public decimal? EstimatedMinutes { get; set; }

    /// <summary>
    /// ID of the user assigned to this task.
    /// </summary>
    public Guid? AssignedToUserId { get; set; }

    /// <summary>
    /// Due date for the task.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// ADHD-friendly quick win flag.
    /// </summary>
    public bool IsQuickWin { get; set; } = false;

    /// <summary>
    /// Task creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ID of the user who created this task.
    /// </summary>
    public Guid CreatedByUserId { get; set; }

    /// <summary>
    /// Last modified timestamp (newly added).
    /// </summary>
    [Column("last_modified_at")]
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property: Collection of comments for this task.
    /// Foreign key: ProjectTaskId (matches FK in ProjectTaskComments)
    /// </summary>
    public virtual ICollection<ProjectTaskComments> Comments { get; set; } = new List<ProjectTaskComments>();

    /// <summary>
    /// FK to ProjectTask.Id (for junction table FK-as-PK pattern).
    /// This is used by configuration files expecting FK-as-PK pattern.
    /// </summary>
    public Guid ProjectTaskId { get; set; } // Default to same as Id
}