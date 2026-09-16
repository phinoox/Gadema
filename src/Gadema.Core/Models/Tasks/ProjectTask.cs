// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

using Gadema.Core.Models.Base.Projects;

namespace Gadema.Core.Models.Tasks;

/// <summary>
/// Represents a task in the project workflow.
/// A component that belongs to a Project and has its own identity anchor.
/// </summary>
[ModelDependency(typeof(Project), typeof(ContentMetaInfo))]
public class ProjectTask
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // --- Identity Anchor (The "Soul") ---
    [Required]
    public Guid MetaInfoId { get; set; }

    [ForeignKey("MetaInfoId")]
    public virtual ProjectTaskMetaInfo MetaInfo { get; set; } = null!;

    // --- Contextual Properties (The "Body" / Component) ---

    /// <summary>
    /// The project this task belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!;

    /// <summary>
    /// User assigned to this task.
    /// </summary>
    public Guid? AssignedToUserId { get; set; }

    /// <summary>
    /// The deadline for the task.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// User who created this task.
    ///</summary>
    public Guid CreatedByUserId { get; set; }

    // Navigation property: Collection of comments for this task.
    public virtual ICollection<ProjectTaskComment> Comments { get; set; } = new List<ProjectTaskComment>();
}