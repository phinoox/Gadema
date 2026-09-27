// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Enums;

/// <summary>
/// Defines the various stages of a story outline in its lifecycle.
/// </summary>
public enum OutlineStatusEnum
{
    /// <summary>The outline is currently being drafted and is not yet complete.</summary>
    DraftOutline = 0,
    
    /// <summary>The outline has been reviewed and finalized.</summary>
    Finalized = 1,
    
    /// <summary>The outline has been published for public or wider viewing.</summary>
    Published = 2,
    
    /// <summary>The outline has been moved to an archive state.</summary>
    Archived = 3
}