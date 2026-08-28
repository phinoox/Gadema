// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Enums;

/// <summary>
/// View mode for content items (PrivateWriting vs Presentation).
/// </summary>
public enum ViewModeEnum
{
    /// <summary>Private Writing (Admin interface)</summary>
    PrivateWriting = 0,
    
    /// <summary>Presentation (Public view)</summary>
    Presentation = 1
}