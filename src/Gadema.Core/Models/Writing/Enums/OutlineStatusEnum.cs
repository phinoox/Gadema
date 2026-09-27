// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Writing.Enums;

/// <summary>
/// Defines the lifecycle stages of a story outline.
/// </summary>
public enum OutlineStatusEnum
{
    /// <summary>An initial, evolving version of the story structure.</summary>
    DraftOutline = 0,
    
    /// <summary>The narrative structure has been completed and confirmed.</summary>
    Finalized = 1,
    
    /// <summary>The outline is ready for public consumption or official use.</summary>
    Published = 2,
    
    /// <summary>A previously active outline that is no longer in primary use.</summary>
    Archived = 3
}