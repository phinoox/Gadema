using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gadema.Core.Models.Base.MetaInfo;
using Gadema.Core.Models.Projects;

namespace Gadema.Core.Models.Tasks;

/// <summary>
/// Identity anchor for a ProjectTask. 
/// Holds the core identity properties of the task.
/// </summary>
[ModelDependency(typeof(ProjectTask), typeof(ContentMetaInfo))]
public class ProjectTaskMetaInfo : BaseMetaInfo
{
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }
    /// <summary>
    /// The status of the task (e.g., Backlog, InProgress, Review, Done).
    /// </summary>
    public TaskStatusEnum Status { get; set; }

    /// <summary>
    /// Task priority (e.g., High, Medium, Low).
    /// </summary>
    public TaskPriorityEnum Priority { get; set; }

    /// <summary>
    /// Difficulty level of the task (e.g., Easy, Medium, Hard).
    /// </summary>
    public TaskDifficultyEnum Difficulty { get; set; }

    /// <summary>
    /// Estimated time in minutes to complete the task.
    /// </summary>
    public decimal? EstimatedMinutes { get; set; }

    /// <summary>
    /// ADHD-friendly quick win flag.
    /// </summary>
    public bool IsQuickWin { get; set; }
}
