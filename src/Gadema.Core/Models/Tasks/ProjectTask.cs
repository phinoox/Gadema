// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

using Gadema.Core.Models.Base.Projects;

namespace Gadema.Core.Models.Tasks;

/// <summary>
/// Represents a discrete unit of work within a project's workflow.
/// A task is anchored by its <see cref="ProjectTaskMetaInfo"/> and is associated with a specific <see cref="Project"/>.
/// </summary>
[ModelDependency(typeof(Project), typeof(ContentMetaInfo))]
public class ProjectTask
{
    /// <summary>
    /// Unique identifier for the task.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    // --- Identity Anchor (The "Soul") ---
    /// <summary>
    /// The ID of the associated identity anchor.
    /// </summary>
    [Required]
    public Guid MetaInfoId { get; set; }

    /// <summary>
    /// Navigation property for the task's identity anchor containing status and priority metadata.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ProjectTaskMetaInfo MetaInfo { get; set; } = null!;

    // --- Contextual Properties (The "Body" / Component) ---
    /// <summary>
    /// The ID of the project this task is associated with.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Navigation property for the parent project.
    /// </summary>
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!;

    /// <summary>
    /// The ID of the user currently assigned to this task.
    /// </summary>
    public Guid? AssignedToUserId { get; set; }

    /// <summary>
    /// The target deadline for completing the task.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// The ID of the user who created this task.
    /// </summary>
    public Guid CreatedByUserId { get; set; }

    /// <summary>
    /// Collection of comments associated with this task.
    /// </summary>
    public virtual ICollection<ProjectTaskComment> Comments { get; set; } = new List<ProjectTaskComment>();
}