namespace Gadema.Core.Models.Base.Projects.Enums;

/// <summary>
/// Project difficulty levels for task estimation and workload management.
/// </summary>
public enum ProjectDifficultyEnum
{
    /// <summary>
    /// Simple tasks, straightforward implementation, minimal complexity.
    /// </summary>
    [Display(Name = "Easy")]
    Easy = 0,

    /// <summary>
    /// Moderate difficulty, requires some planning and coordination.
    /// </summary>
    [Display(Name = "Medium")]
    Medium = 1,

    /// <summary>
    /// Complex tasks, extensive research or implementation required.
    /// </summary>
    [Display(Name = "Hard")]
    Hard = 2,
}
