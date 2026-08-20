// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Dtos.Search;

/// <summary>
/// DTO for searching content items.
/// </summary>
public class SearchContentItemsDto
{
    /// <summary>
    /// Search query (title, description, slug) (required).
    /// </summary>
    [Required, MaxLength(512)]
    public string Query { get; set; } = "";
    
    /// <summary>
    /// Filter by content type (optional).
    /// </summary>
    [EnumDataType(typeof(ContentTypeEnum))]
    public ContentTypeEnum? ContentType { get; set; } = null!;
    
    /// <summary>
    /// Only published items (optional).
    /// </summary>
    public bool PublishedOnly { get; set; } = false;
}