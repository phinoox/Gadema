using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Enums;

/// <summary>
/// Task priority levels for workflow management.
/// </summary>
public enum TaskPriorityEnum
{
    /// <summary>
    /// High priority tasks requiring immediate attention.
    /// </summary>
    [Display(Name = "High")]
    High = 0,

    /// <summary>
    /// Medium priority tasks with standard timeline.
    /// </summary>
    [Display(Name = "Medium")]
    Medium = 1,

    /// <summary>
    /// Low priority tasks that can be deferred.
    /// </summary>
    [Display(Name = "Low")]
    Low = 2,
}
