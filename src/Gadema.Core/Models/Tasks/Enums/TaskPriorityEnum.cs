namespace Gadema.Core.Models.Tasks.Enums;

/// <summary>
/// Defines the priority level for tasks within a project workflow.
/// </summary>
public enum TaskPriorityEnum
{
    /// <summary>
    /// High importance; requires immediate attention or takes precedence over other work.
    /// </summary>
    [Display(Name = "High")]
    High = 0,

    /// <summary>
    /// Standard priority; should be completed within the normal workflow timeline.
    /// </summary>
    [Display(Name = "Medium")]
    Medium = 1,

    /// <summary>
    /// Low importance; tasks that can be deferred or addressed when time permits.
    /// </summary>
    [Display(Name = "Low")]
    Low = 2,
}

