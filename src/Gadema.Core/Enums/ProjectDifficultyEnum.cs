using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Enums;

/// <summary>
/// Represents the estimated difficulty level of a project, used for workload management and planning.
/// </summary>
public enum ProjectDifficultyEnum
{
    /// <summary>
    /// Simple projects with minimal complexity and straightforward implementation.
    /// </summary>
    [Display(Name = "Easy")]
    Easy = 0,

    /// <summary>
    /// Projects of moderate difficulty requiring some planning and coordination.
    /// </summary>
    [Display(Name = "Medium")]
    Medium = 1,

    /// <summary>
    /// Complex projects involving extensive research or implementation requirements.
    /// </summary>
    [Display(Name = "Hard")]
    Hard = 2,
}

