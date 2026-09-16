// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Base.Enums;

/// <summary>
/// Content status for content items.
/// </summary>
public enum ContentStatusEnum
{
    /// <summary>Draft</summary>
    Draft = 0,
    
    /// <summary>In Progress</summary>
    InProgress = 1,
    
    /// <summary>Published</summary>
    Published = 2,
    
    /// <summary>Archived</summary>
    Archived = 3,
    
    /// <summary>Under Review</summary>
    UnderReview = 4,
    
    /// <summary>Completed</summary>
    Completed = 5
}