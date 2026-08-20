// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Enums;

/// <summary>
/// Task status for workflow management.
/// </summary>
public enum TaskStatusEnum
{
    /// <summary>Backlog</summary>
    Backlog = 0,
    
    /// <summary>In Progress</summary>
    InProgress = 1,
    
    /// <summary>Review</summary>
    Review = 2,
    
    /// <summary>Done</summary>
    Done = 3
}