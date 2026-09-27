using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Enums;

/// <summary>
/// Represents the importance or urgency level of a task within a workflow.
/// </summary>
public enum TaskPriorityEnum
{
    /// <summary>
    /// High priority tasks that require immediate attention or have tight deadlines.
    /// </summary>
    [Display(Name = "High")]
    High = 0,

    /// <summary>
    /// Medium priority tasks with a standard timeline and moderate urgency.
    /// </summary>
    [Display(Name = "Medium")]
    Medium = 1,

    /// <summary>
    /// Low priority tasks that can be deferred or addressed as time permits.
    /// </summary>
    [Display(Name = "Low")]
    Low = 2,
}

