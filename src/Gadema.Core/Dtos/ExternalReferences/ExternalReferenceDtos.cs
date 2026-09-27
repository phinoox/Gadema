namespace Gadema.Core.Dtos.ExternalReferences;

using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Models.Base.Infrastructure.Enums;


/// <summary>
/// Data transfer object for creating a new external reference (e.g., a website, book, or article).
/// </summary>
public class ExternalReferenceCreateDto
{
    /// <summary>
    /// The metadata required to establish the reference's identity.
    /// </summary>
    [Required]
    public BaseMetaInfoCreateData ContentMetaInfo { get; set; } = new();

    /// <summary>
    /// The type of external reference (e.g., Website, Book, Article).
    /// </summary>
    [Required]
    public ExternalReferenceTypeEnum ReferenceType { get; set; }

    /// <summary>
    /// The URL for the external resource.
    /// </summary>
    [Required, MaxLength(2048)]
    public string Url { get; set; } = "";

    /// <summary>
    /// A descriptive title for the reference.
    /// </summary>
    [MaxLength(128)]
    public string? Title { get; set; }

    /// <summary>
    /// The name of the author or creator of the resource.
    /// </summary>
    [MaxLength(128)]
    public string? Author { get; set; }

    /// <summary>
    /// Additional notes or context regarding the reference.
    /// </summary>
    [MaxLength(4096)]
    public string? Notes { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing external reference.
/// </summary>
public class ExternalReferenceUpdateDto
{
    /// <summary>
    /// The identity payload used by the sync strategy to update meta-information.
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    /// <summary>
    /// The updated reference type.
    /// </summary>
    public ExternalReferenceTypeEnum? ReferenceType { get; set; }

    /// <summary>
    /// The updated URL for the resource.
    /// </summary>
    [MaxLength(2048)] public string? Url { get; set; }

    /// <summary>
    /// The updated title of the reference.
    /// </summary>
    [MaxLength(128)] public string? Title { get; set; }

    /// <summary>
    /// The updated author name.
    /// </summary>
    [MaxLength(128)] public string? Author { get; set; }

    /// <summary>
    /// The updated notes or context.
    /// </summary>
    [MaxLength(4096)] public string? Notes { get; set; }

    /// <summary>
    /// Whether to update the active status of this reference.
    /// </summary>
    public bool? IsActive { get; set; }
}

/// <summary>
/// Represents an external reference, including its identity and domain-specific properties.
/// </summary>
public class ExternalReferenceResponseDto
{
    /// <summary>
    /// The unique identifier of the reference record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the associated meta-information anchor.
    /// </summary>
    public Guid MetaInfoId { get; set; }
    
    // Denormalized Identity Properties
    /// <summary>
    /// The title of the reference.
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// The URL-friendly slug for the reference.
    /// </summary>
    public string Slug { get; set; } = "";

    /// <summary>
    /// Indicates if this reference is public.
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// The timestamp when the reference was created in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // Domain Properties
    /// <summary>
    /// The type of external resource.
    /// </summary>
    public ExternalReferenceTypeEnum ReferenceType { get; set; }

    /// <summary>
    /// The URL for the resource.
    /// </summary>
    public string Url { get; set; } = "";

    /// <summary>
    /// The name of the author or creator.
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// Additional notes regarding the reference.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Indicates if this reference is currently active.
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// A collection of external references, typically used for paginated lists.
/// </summary>
public class ExternalReferenceListResponseDto
{
    /// <summary>
    /// The list of retrieved external references.
    /// </summary>
    public IEnumerable<ExternalReferenceResponseDto> Items { get; set; } = Enumerable.Empty<ExternalReferenceResponseDto>();

    /// <summary>
    /// Total number of references found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}