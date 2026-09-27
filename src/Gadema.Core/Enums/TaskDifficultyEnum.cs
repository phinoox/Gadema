// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Enums;

/// <summary>
/// Represents the estimated difficulty level of a task, designed to be ADHD-friendly for better estimation and management.
/// </summary>
public enum TaskDifficultyEnum
{
    /// <summary>A simple task that can be completed quickly with minimal effort.</summary>
    Easy = 0,
    
    /// <summary>A moderate task requiring some sustained attention and planning.</summary>
    Medium = 1,
    
    /// <summary>A complex task that may require significant mental energy and focus.</summary>
    Hard = 2
}