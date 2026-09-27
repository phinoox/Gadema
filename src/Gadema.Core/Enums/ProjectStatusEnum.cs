// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Enums;

/// <summary>
/// Represents the current lifecycle stage of a project.
/// </summary>
public enum ProjectStatusEnum
{
    /// <summary>The project is in initial draft stage.</summary>
    Draft = 0,
    
    /// <summary>The project is currently being actively developed.</summary>
    InProgress = 1,
    
    /// <summary>The project has been completed and published.</summary>
    Published = 2,
    
    /// <summary>The project has been moved to an archive state.</summary>
    Archived = 3,

    /// <summary>The project has been marked for deletion.</summary>
    Deleted = 4
}