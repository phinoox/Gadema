// =============================================================================
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Enums;

namespace Gadema.Core.Dtos.Tags;

/// <summary>
/// DTO for creating a tag on a content item.
/// </summary>
public class TagCreateDto
{
    /// <summary>
    /// ContentMetaInfo data for the tag's identity (used as the tag's own identity).
    /// </summary>
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    /// <summary>
    /// ID of the content item this tag is being attached to.
    /// </summary>
    [Required] public Guid ContentItemId { get; set; }

    /// <summary>
    /// The tag name (e.g., "Dragon", "Hero", "SideQuest").
    /// Must be unique within the project for a given content item.
    /// </summary>
    [Required, MaxLength(128)] public string TagName { get; set; } = "";

    /// <summary>
    /// URL-friendly slug for the tag (auto-generated from Name if null).
    /// </summary>
    [MaxLength(128)] public string? Slug { get; set; }

    /// <summary>
    /// Hex color code for visual representation.
    /// E.g., "#FF5733" or null for default color.
    /// </summary>
    [MaxLength(8)] public string? ColorHex { get; set; }
}

/// <summary>
/// DTO for removing a tag from a content item (partial update).
/// </summary>
public class TagUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// Remove this specific tag by setting it to null.
    /// </summary>
    public string? TagNameToRemove { get; set; }

    /// <summary>
    /// ContentMetaInfo fields (nullable — omit to keep current values).
    /// </summary>
    public MetaInfoUpdateData? ContentMetaInfo { get; set; }

    [MaxLength(128)] public string? ColorHex { get; set; }
}

/// <summary>
/// Response DTO for a single tag on a content item. Inherits ContentMetaInfo state.
/// </summary>
public class TagResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// The human-readable name of the tag.
    /// </summary>
    [Required, MaxLength(128)] public string Name { get; set; } = "";

    /// <summary>
    /// URL-friendly slug (used for UI routing).
    /// </summary>
    [MaxLength(128)] public string Slug { get; set; } = "";

    /// <summary>
    /// Hex color code for visual representation.
    /// </summary>
    [MaxLength(8)] public string? ColorHex { get; set; }

    /// <summary>
    /// The content item this tag is attached to.
    /// </summary>
    public Guid ContentItemId { get; set; }

    /// <summary>
    /// Title of the content item (denormalized for convenience).
    /// </summary>
    [MaxLength(256)] public string? ContentTypeTitle { get; set; }
}

/// <summary>
/// List response for tags on a single content item.
/// </summary>
public class TagListResponseDto
{
    /// <summary>
    /// The parent content item ID (for context).
    /// </summary>
    public Guid ContentItemId { get; set; }

    public IEnumerable<TagResponseDto> Tags { get; set; } = Enumerable.Empty<TagResponseDto>();

    public int TotalCount { get; set; }
}

/// <summary>
/// Request DTO for adding multiple tags at once.
/// </summary>
public class AddTagsDto
{
    /// <summary>
    /// The content item ID to attach these tags to.
    /// </summary>
    [Required] public Guid ContentItemId { get; set; }

    /// <summary>
    /// List of tag names to add. Each must exist in the system.
    /// </summary>
    [Required] public IEnumerable<string> TagNames { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Optional: hex color codes corresponding to each tag name (by order).
    /// If omitted, tags keep their existing colors or use defaults.
    /// </summary>
    [MaxLength(128)] public IEnumerable<string>? ColorHexes { get; set; }
}

/// <summary>
/// List response for all known tags in a project (for autocomplete/filters).
/// </summary>
public class TagFilterResponseDto
{
    /// <summary>
    /// The tag's name.
    /// </summary>
    [Required, MaxLength(128)] public string Name { get; set; } = "";

    /// <summary>
    /// URL-friendly slug.
    /// </summary>
    [MaxLength(128)] public string Slug { get; set; } = "";

    /// <summary>
    /// Hex color code. Null means "no color assigned" (use default).
    /// </summary>
    [MaxLength(8)] public string? ColorHex { get; set; }

    /// <summary>
    /// Number of content items tagged with this tag in the project.
    /// </summary>
    public int UsageCount { get; set; }
}