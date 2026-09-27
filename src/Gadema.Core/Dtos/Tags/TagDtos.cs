/// <summary>
/// Data transfer object for creating a new tag on a specific content item.
/// </summary>
public class TagCreateDto
{
    /// <summary>
    /// The metadata required to establish the tag's identity (e.g., title, slug).
    /// </summary>
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    /// <summary>
    /// The unique identifier of the content item this tag is being attached to.
    /// </summary>
    [Required] public Guid ContentItemId { get; set; }

    /// <summary>
    /// The display name for the tag (e.g., "Dragon", "Hero"). 
    /// Must be unique within the project for a given content item.
    /// </summary>
    [Required, MaxLength(128)] public string TagName { get; set; } = "";

    /// <summary>
    /// A URL-friendly slug for the tag (auto-generated from Name if null).
    /// </summary>
    [MaxLength(128)] public string? Slug { get; set; }

    /// <summary>
    /// Hexadecimal color code for visual representation (e.g., "#FF5733").
    /// </summary>
    [MaxLength(8)] public string? ColorHex { get; set; }
}

/// <summary>
/// Data transfer object used to remove or update a tag on a content item.
/// </summary>
public class TagUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// The name of the tag to be removed (set this value to trigger removal).
    /// </summary>
    public string? TagNameToRemove { get; set; }

    /// <summary>
    /// Metadata fields for updating the tag's identity (e.g., Name, Slug).
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    /// <summary>
    /// The updated hex color code for visual representation.
    /// </summary>
    [MaxLength(128)] public string? ColorHex { get; set; }
}

/// <summary>
/// Represents a single tag attached to a content item, including its metadata.
/// Inherits standard meta-information state.
/// </summary>
public class TagResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// The human-readable display name of the tag.
    /// </summary>
    [Required, MaxLength(128)] public string Name { get; set; } = "";

    /// <summary>
    /// The URL-friendly slug used for UI routing and identification.
    /// </summary>
    [MaxLength(128)] public string Slug { get; set; } = "";

    /// <summary>
    /// Hexadecimal color code for the tag's visual representation.
    /// </summary>
    [MaxLength(8)] public string? ColorHex { get; set; }

    /// <summary>
    /// The unique identifier of the content item this tag is attached to.
    /// </summary>
    public Guid ContentItemId { get; set; }

    /// <summary>
    /// Denormalized title of the parent content item for UI convenience.
    /// </summary>
    [MaxLength(256)] public string? ContentTypeTitle { get; set; }
}

/// <summary>
/// A collection of tags associated with a specific content item.
/// </summary>
public class TagListResponseDto
{
    /// <summary>
    /// The ID of the parent content item.
    /// </summary>
    public Guid ContentItemId { get; set; }

    /// <summary>
    /// The list of tags found on this content item.
    /// </summary>
    public IEnumerable<TagResponseDto> Tags { get; set; } = Enumerable.Empty<TagResponseDto>();

    /// <summary>
    /// Total number of tags on this item.
    /// </summary>
    public int TotalCount { get; set; }
}

/// <summary>
/// Request DTO for adding multiple new or existing tags to a content item at once.
/// </summary>
public class AddTagsDto
{
    /// <summary>
    /// The unique identifier of the content item receiving these tags.
    /// </summary>
    [Required] public Guid ContentItemId { get; set; }

    /// <summary>
    /// A list of tag names to be added/linked.
    /// </summary>
    [Required] public IEnumerable<string> TagNames { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Optional: corresponding hex color codes for the tags provided in [TagNames].
    /// If omitted, existing colors or defaults will be used.
    /// </summary>
    [MaxLength(128)] public IEnumerable<string>? ColorHexes { get; set; }
}

/// <summary>
/// Represents a tag's metadata used for filtering and autocomplete in the UI.
/// </summary>
public class TagFilterResponseDto
{
    /// <summary>
    /// The display name of the tag.
    /// </summary>
    [Required, MaxLength(128)] public string Name { get; set; } = "";

    /// <summary>
    /// The URL-friendly slug for the tag.
    /// </summary>
    [MaxLength(128)] public string Slug { get; set; } = "";

    /// <summary>
    /// Hexadecimal color code for visual representation.
    /// </summary>
    [MaxLength(8)] public string? ColorHex { get; set; }

    /// <summary>
    /// The number of content items in the current project that are tagged with this tag.
    /// Useful for ranking popularity in autocomplete.
    /// </summary>
    public int UsageCount { get; set; }
}