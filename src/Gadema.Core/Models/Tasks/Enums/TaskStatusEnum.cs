// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Tasks.Enums;

/// <summary>
/// Defines the lifecycle stages of a task within a project workflow.
/// </summary>
public enum TaskStatusEnum
{
    /// <summary>The task is identified but not yet scheduled or started.</summary>
    Backlog = 0,
    
    /// <summary>The task is currently being actively worked on.</summary>
    InProgress = 1,
    
    /// <summary>The work is completed and awaiting verification or feedback.</summary>
    Review = 2,
    
    /// <summary>The task has been successfully completed and verified.</summary>
    Done = 3
}