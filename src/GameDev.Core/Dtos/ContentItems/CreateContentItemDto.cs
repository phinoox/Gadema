// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Dtos.ContentItems;

/// <summary>
/// DTO for creating a new content item.
/// </summary>
public class CreateContentItemDto
{
    /// <summary>
    /// ID of the project this content item belongs to (required).
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Content type (Character, World, Mechanic, etc.) (required).
    /// </summary>
    [EnumDataType(typeof(ContentTypeEnum)), Required, Display(Name = "Content Type")]
    public ContentTypeEnum ContentType { get; set; }
    
    /// <summary>
    /// Title of the content item (required).
    /// </summary>
    [MaxLength(128), Required, Display(Name = "Title")]
    public string Title { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the content item (optional, auto-generated if not provided).
    /// </summary>
    [MaxLength(128)]
    public string? Slug { get; set; } = null!;
    
    /// <summary>
    /// Full description (Markdown/HTML) (optional).
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; } = null!;
    
    /// <summary>
    /// Short description for search/filtering (optional).
    /// </summary>
    [MaxLength(4096)]
    public string? ShortDesc { get; set; } = null!;
}