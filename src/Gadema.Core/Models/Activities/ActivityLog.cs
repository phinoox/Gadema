// =============================================================================
using Gadema.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Represents an activity log entry for project events.
/// </summary>
[DependencyResolver.ModelDependency(typeof(Project))]
public class ActivityLog
{
    /// <summary>
    /// Unique identifier for the activity log entry.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// ID of the project this activity belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// ID of the user who performed the action (null if automated).
    /// </summary>
    public Guid? UserId { get; set; }  // Null if automated action

    /// <summary>
    /// Event type (e.g., "ContentCreated", "TaskCompleted").
    /// </summary>
    [MaxLength(128)]
    public string EventType { get; set; } = "";  // e.g., "ContentCreated", "TaskCompleted"

    /// <summary>
    /// ID of the related entity (nullable FK).
    /// </summary>
    public Guid? RelatedEntityId { get; set; }  // Nullable FK to related entity

    /// <summary>
    /// Type of related entity.
    /// </summary>
    public int RelatedEntityType { get; set; }  // Enum: ContentItem, Task, etc.

    /// <summary>
    /// Activity title.
    /// </summary>
    [MaxLength(512)]
    public string? Title { get; set; }

    /// <summary>
    /// Activity description.
    /// </summary>
    [MaxLength(2048)]
    public string? Description { get; set; }

    /// <summary>
    /// Timestamp when the activity occurred.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //Navigation Properties
    public virtual Project Project { get; set; }
    public virtual User? User { get; set; }
}