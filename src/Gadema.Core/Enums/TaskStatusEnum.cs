// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Enums;

/// <summary>
/// Represents the current status of a task within a workflow lifecycle.
/// </summary>
public enum TaskStatusEnum
{
    /// <summary>The task is in the backlog and has not yet been started.</summary>
    Backlog = 0,
    
    /// <summary>The task is currently being actively worked on.</summary>
    InProgress = 1,
    
    /// <summary>The task is completed and awaiting review or verification.</summary>
    Review = 2,
    
    /// <summary>The task has been successfully completed.</summary>
    Done = 3
}