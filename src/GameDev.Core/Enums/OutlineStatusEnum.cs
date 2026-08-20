// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Enums;

/// <summary>
/// Outline status for story outlines.
/// </summary>
public enum OutlineStatusEnum
{
    /// <summary>Draft Outline</summary>
    DraftOutline = 0,
    
    /// <summary>Finalized</summary>
    Finalized = 1,
    
    /// <summary>Published</summary>
    Published = 2,
    
    /// <summary>Archived</summary>
    Archived = 3
}