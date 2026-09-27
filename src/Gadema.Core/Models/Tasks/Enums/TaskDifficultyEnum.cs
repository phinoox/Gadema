// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Tasks.Enums;

/// <summary>
/// Defines the perceived difficulty level of a task. Designed to be ADHD-friendly by providing clear, actionable complexity tiers.
/// </summary>
public enum TaskDifficultyEnum
{
    /// <summary>Low cognitive load; can be completed quickly with minimal effort.</summary>
    Easy = 0,
    
    /// <summary>Moderate cognitive load; requires focused attention and some time.</summary>
    Medium = 1,
    
    /// <summary>High cognitive load; requires significant mental energy or multiple steps to complete.</summary>
    Hard = 2
}