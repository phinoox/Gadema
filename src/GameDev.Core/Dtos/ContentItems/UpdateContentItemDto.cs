// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Dtos.ContentItems;

/// <summary>
/// DTO for updating a content item.
/// </summary>
public class UpdateContentItemDto
{
    /// <summary>
    /// Updated description (optional).
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; } = null!;
    
    /// <summary>
    /// Publish/unpublish the content item (optional).
    /// </summary>
    public bool? Published { get; set; } = null!;
    
    /// <summary>
    /// Switch between PrivateWriting and Presentation view modes (optional).
    /// </summary>
    [EnumDataType(typeof(ViewModeEnum))]
    public ViewModeEnum? ViewMode { get; set; } = null!;
}