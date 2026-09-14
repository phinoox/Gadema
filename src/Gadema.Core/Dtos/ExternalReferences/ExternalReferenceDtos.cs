// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Content.ExternalReferences;

/// <summary>
/// DTO for creating an external reference (link to outside resources).
/// </summary>
public class ExternalReferenceCreateDto
{
    /// <summary>
    /// MetaInfo data for the reference's identity.
    /// </summary>
    [Required] public MetaInfoCreateData CreateData { get; set; } = new();

    /// <summary>
    /// ID of the content item this reference belongs to.
    /// </summary>
    [Required] public Guid ContentItemId { get; set; }

    /// <summary>
    /// The external URL (required).
    /// </summary>
    [Required, MaxLength(2048)] public string Url { get; set; } = "";

    /// <summary>
    /// Title of the external resource.
    /// </summary>
    [MaxLength(256)] public string? Title { get; set; }

    /// <summary>
    /// Type: 0=Document, 1=Image, 2=Video, 3=Audio, 4=Tool/Software, 5=Other.
    /// </summary>
    [Range(0, 5)] public int Type { get; set; } = 0;

    /// <summary>
    /// Optional: thumbnail URL (auto-generated from Url if null).
    /// </summary>
    [MaxLength(2048)] public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Optional: description of why this reference is relevant.
    /// </summary>
    [MaxLength(1024)] public string? Description { get; set; }
}

/// <summary>
/// DTO for updating an external reference (partial update).
/// </summary>
public class ExternalReferenceUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// MetaInfo fields (nullable — omit to keep current values).
    /// </summary>
    public MetaInfoUpdateData? MetaInfo { get; set; }

    [MaxLength(2048)] public string? Url { get; set; }
    [MaxLength(256)] public string? Title { get; set; }
    [Range(0, 5)] public int? Type { get; set; }
    [MaxLength(2048)] public string? ThumbnailUrl { get; set; }
    [MaxLength(1024)] public string? Description { get; set; }
}

/// <summary>
/// Response DTO for an external reference. Inherits MetaInfo state.
/// </summary>
public class ExternalReferenceResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// ID of the content item this reference belongs to.
    /// </summary>
    [Required] public Guid ContentItemId { get; set; }

    /// <summary>
    /// The external URL.
    /// </summary>
    [MaxLength(2048)] public string Url { get; set; } = "";

    /// <summary>
    /// Title of the resource (denormalized for convenience).
    /// </summary>
    [MaxLength(256)] public string? Title { get; set; }

    /// <summary>
    /// Type: Document, Image, Video, Audio, Tool/Software, Other.
    /// </summary>
    [Display(Name = "Type")]
    public int Type { get; set; }

    [MaxLength(2048)] public string? ThumbnailUrl { get; set; }
    [MaxLength(1024)] public string? Description { get; set; }

    /// <summary>
    /// Human-readable type name.
    /// </summary>
    [Display(Name = "Type Name")]
    public string TypeName { get; set; } = "";
}

/// <summary>
/// List response for external references on a content item.
/// </summary>
public class ExternalReferenceListResponseDto
{
    /// <summary>
    /// The parent content item ID.
    /// </summary>
    public Guid ContentItemId { get; set; }

    public IEnumerable<ExternalReferenceResponseDto> Items { get; set; } = Enumerable.Empty<ExternalReferenceResponseDto>();

    public int TotalCount { get; set; }
}

/// <summary>
/// Request DTO for removing external references.
/// </summary>
public class ExternalReferenceRemoveDto : UpdateRequestDto
{
    /// <summary>
    /// IDs of references to remove (comma-separated or JSON array).
    /// Pass null/empty string to clear ALL references on this content item.
    /// </summary>
    [MaxLength(4096)] public string? ReferenceIdsToRemove { get; set; }

    /// <summary>
    /// Set to true to permanently delete (hard delete), false for soft-delete.
    /// Default is false (soft-delete).
    /// </summary>
    public bool HardDelete { get; set; } = false;
}