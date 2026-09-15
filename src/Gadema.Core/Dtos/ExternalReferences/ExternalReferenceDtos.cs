using System.ComponentModel.DataAnnotations;


namespace Gadema.Core.Dtos.ExternalReferences;

/// <summary>
/// DTO for creating an external reference.
/// </summary>
public class ExternalReferenceCreateDto
{
    /// <summary>
    /// The nested identity payload.
    /// </summary>
    [Required]
    public BaseMetaInfoCreateData ContentMetaInfo { get; set; } = new();

    [Required]
    public ExternalReferenceTypeEnum ReferenceType { get; set; }

    [Required, MaxLength(2048)]
    public string Url { get; set; } = "";

    [MaxLength(128)]
    public string? Title { get; set; }

    [MaxLength(128)]
    public string? Author { get; set; }

    [MaxLength(4096)]
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for updating an external reference.
/// </summary>
public class ExternalReferenceUpdateDto
{
    /// <summary>
    /// The nested identity payload for the sync strategy.
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    public ExternalReferenceTypeEnum? ReferenceType { get; set; }
    [MaxLength(2048)] public string? Url { get; set; }
    [MaxLength(128)] public string? Title { get; set; }
    [MaxLength(128)] public string? Author { get; set; }
    [MaxLength(4096)] public string? Notes { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// Response DTO for an external reference.
/// Denormalizes identity properties from the MetaInfo anchor.
/// </summary>
public class ExternalReferenceResponseDto
{
    public Guid Id { get; set; }
    public Guid MetaInfoId { get; set; }
    
    // Denormalized Identity Properties
    public string Title { get; set; } = "";
    public string Slug { get; set; } = "";
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }

    // Domain Properties
    public ExternalReferenceTypeEnum ReferenceType { get; set; }
    public string Url { get; set; } = "";
    public string? Author { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// List response for external references.
/// </summary>
public class ExternalReferenceListResponseDto
{
    public IEnumerable<ExternalReferenceResponseDto> Items { get; set; } = Enumerable.Empty<ExternalReferenceResponseDto>();
    public int TotalCount { get; set; }
}