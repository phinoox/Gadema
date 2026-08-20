// =============================================================================
// EngineFieldMapping - Entity for engine field name mappings per content type
// =============================================================================

using System.ComponentModel.DataAnnotations;
using GameDev.Core.Enums;

namespace GameDev.Core.Models;

/// <summary>
/// Maps content fields to their corresponding engine property names.
/// Example: ContentItem.Title -> "Player.Name" in Unreal Engine.
/// </summary>
public class EngineFieldMapping
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
    
    [EnumDataType(typeof(ContentTypeEnum))]
    public ContentTypeEnum ContentType { get; set; }
    
    [MaxLength(128), Required, Display(Name = "Content Field Name")]
    public string ContentFieldName { get; set; } = "";
    
    [MaxLength(256)]
    public string? EngineFieldName { get; set; }  // e.g., "Player.Health"
    
    public bool IsRequired { get; set; } = false;
    
    public int DataType { get; set; }  // Nullable: Int, Float, String, Boolean
    
    [MaxLength(1024)]
    public string? Description { get; set; }
}
