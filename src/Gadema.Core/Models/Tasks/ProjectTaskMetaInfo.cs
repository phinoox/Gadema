using Gadema.Core.Models.Base.Projects;
using Gadema.Core.Models.Tasks.Enums;

namespace Gadema.Core.Models.Tasks;

/// <summary>
/// Serves as the identity anchor for a <see cref="ProjectTask"/>.
/// This entity holds the core status, priority, and metadata required to manage task workflows.
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class ProjectTaskMetaInfo : BaseMetaInfo
{
    /// <summary>
    /// The ID of the project this task belongs to.
    /// </summary>
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }

    /// <summary>
        /// Navigation property for the associated project.
    /// </summary>
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }

    /// <summary>
    /// The current lifecycle stage of the task (e.g., Backlog, InProgress, Review, Done).
    /// </summary>
    public TaskStatusEnum Status { get; set; }

    /// <summary>
    /// The importance level of the task within the project workflow.
    /// </summary>
    public TaskPriorityEnum Priority { get; set; }

    /// <summary>
    /// The perceived cognitive difficulty of completing the task.
    /// </summary>
    public TaskDifficultyEnum Difficulty { get; set; }

    /// <summary>
    /// Estimated time in minutes required to complete the task.
    /// </summary>
    public decimal? EstimatedMinutes { get; set; }

    /// <summary>
    /// Indicates if this task is categorized as a "Quick Win"—a small, low-effort
    /// achievement designed to boost motivation and momentum.
    /// </summary>
    public bool IsQuickWin { get; set; }
}

