using Gadema.Core.Models.Writing.Enums;


/// <summary>
/// Data transfer object for creating a new lore entry (e.g., world fact, historical event).
/// </summary>
public class LoreEntryCreateDto
{
    /// <summary>
    /// The metadata required to establish the lore entry's identity.
    /// </summary>
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();
    
    /// <summary>
    /// The category of lore (e.g., History, Magic, Geography).
    /// </summary>
    [EnumDataType(typeof(LoreTypeEnum)), Required]
    public LoreTypeEnum LoreType { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing lore entry.
/// </summary>
public class LoreEntryUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// The identity payload used by the sync strategy to update meta-information (e.g., Title, Slug).
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }
    
    /// <summary>
    /// The updated raw text content of the lore entry.
    /// </summary>
    public string? RawText { get; set; }

    /// <summary>
    /// Whether to update the visibility status.
    /// </summary>
    public bool? IsPublic { get; set; }
}

/// <summary>
/// Represents a piece of lore, including its type and content.
/// </summary>
public class LoreEntryResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// The category of the lore entry.
    /// </summary>
    public int LoreType { get; set; }

    /// <summary>
    /// The raw text content of the lore entry.
    /// </summary>
    public string? RawText { get; set; }
}

/// <summary>
/// A collection of lore entries, typically used for paginated lists.
/// </summary>
public class LoreEntryListResponseDto
{
    /// <summary>
    /// The list of retrieved lore entries.
    /// </summary>
    public IEnumerable<LoreEntryResponseDto> Items { get; set; } = Enumerable.Empty<LoreEntryResponseDto>();

    /// <summary>
    /// Total number of lore entries found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}