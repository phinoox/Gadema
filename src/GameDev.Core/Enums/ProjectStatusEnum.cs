// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Enums;

/// <summary>
/// Project status for lifecycle management.
/// </summary>
public enum ProjectStatusEnum
{
    /// <summary>Draft</summary>
    Draft = 0,
    
    /// <summary>In Progress</summary>
    InProgress = 1,
    
    /// <summary>Published</summary>
    Published = 2,
    
    /// <summary>Archived</summary>
    Archived = 3
}