using System.ComponentModel.DataAnnotations;
using Gadema.Core.Enums;

namespace Gadema.Core.Dtos.Identity;

/// <summary>
/// DTO for creating an identity definition.
/// </summary>
public class IdentityDefinitionCreateDto
{
    /// <summary>
    /// The nested identity payload.
    /// </summary>
    [Required]
    public BaseMetaInfoCreateData ContentMetaInfo { get; set; } = new();

    [Required]
    public IdentityDataTypeEnum DataType { get; set; }

    [Required, MaxLength(128)]
    public string Name { get; set; } = "";

    [MaxLength(256)]
    public string? Description { get; set; }

    public bool IsRequired { get; set; } = false;

    [Required]
    public Guid ProjectId { get; set; }
}

/// <summary>
/// DTO for updating an identity definition.
/// </summary>
public class IdentityDefinitionUpdateDto
{
    /// <summary>
    /// The nested identity payload for the sync strategy.
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    public IdentityDataTypeEnum? DataType { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? IsRequired { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// Response DTO for an identity definition.
/// Denormalizes properties from the MetaInfo anchor.
/// </summary>
public class IdentityDefinitionResponseDto
{
    public Guid Id { get; set; }
    public Guid MetaInfoId { get; set; }

    // Denormalized Identity Properties
    public string Title { get; set; } = "";
    public string Slug { get; set; } = "";
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }

    // Domain Properties
    public IdentityDataTypeEnum DataType { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public bool IsRequired { get; set; }
    public Guid ProjectId { get;set; } // Matches the anchor's project
}

/// <summary>
/// List response for identity definitions.
/// </summary>
public class IdentityDefinitionListResponseDto
{
    public IEnumerable<IdentityDefinitionResponseDto> Items { get; set; } = Enumerable.Empty<IdentityDefinitionResponseDto>();
    public int TotalCount { get; set; }
}