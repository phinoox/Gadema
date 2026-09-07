// =============================================================================
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Enums;

namespace Gadema.Core.Dtos.Narrative;

/// <summary>
/// DTO for creating a lore entry.
/// </summary>
public class LoreEntryCreateDto
{
    /// <summary>
    /// ID of the content item this lore belongs to (optional).
    /// </summary>
    public Guid? MetaInfoId { get; set; }

    /// <summary>
    /// ID of the project this lore belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Type of lore (History, Mythology, Geography, etc.).
    /// </summary>
    [EnumDataType(typeof(LoreTypeEnum)), Required]
    public LoreTypeEnum LoreType { get; set; }

    /// <summary>
    /// Title of the lore entry.
    /// </summary>
    [Required, MaxLength(128)]
    public string Title { get; set; } = "";

    /// <summary>
    /// URL-friendly slug.
    /// </summary>
    [MaxLength(128)]
    public string? Slug { get; set; }

    /// <summary>
    /// Content of the lore entry.
    /// </summary>
    [MaxLength(4096)]
    public string? Content { get; set; }
}

/// <summary>
/// Response DTO for a lore entry.
/// </summary>
public class LoreEntryResponseDto
{
    public Guid Id { get; set; }
    public Guid? MetaInfoId { get; set; }
    public Guid ProjectId { get; set; }
    public int LoreType { get; set; }
    public string Title { get; set; } = "";
    public string Slug { get; set; } = "";
    public string? Content { get; set; }
    public bool Published { get; set; }
}

/// <summary>
/// List response for lore entries.
/// </summary>
public class LoreEntryListResponseDto
{
    public IEnumerable<LoreEntryResponseDto> Items { get; set; } = Enumerable.Empty<LoreEntryResponseDto>();
    public int TotalCount { get; set; }
}
