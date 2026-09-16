using System.ComponentModel.DataAnnotations;
using Gadema.Core.Models.Base.MetaInfo;

namespace Gadema.Core.Dtos.Identity;

/// <summary>
/// DTO for creating an identity value.
/// </summary>
public class IdentityValueCreateDto
{
    /// <summary>
    /// The nested identity payload.
    /// </summary>
    [Required]
    public BaseMetaInfoCreateData ContentMetaInfo { get; set; } = new();

    /// <summary>
    /// The ID of the parent IdentityDefinition this value belongs to.
    /// </summary>
    [Required]
    public Guid IdentityDefinitionId { get; set; }

    [Required, MaxLength(128)] 
    public string Name { get; set; } = "";

    [MaxLength(4096)] 
    public string? Description { get; set; }

    public int OrderIndex { get; set; } = 0;
    public bool IsDefault { get; set; } = false;
}

/// <summary>
/// DTO for updating an identity value.
/// </summary>
public class IdentityValueUpdateDto
{
    /// <summary>
    /// The nested identity payload for the sync strategy.
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    [MaxLength(128)] 
    public string? Name { get; set; }

    [MaxLength(4096)] 
    public string? Description { get; set; }

    public int? OrderIndex { get; set; }
    public bool? IsDefault { get; set; }
}

/// <summary>
/// Response DTO for an identity value.
/// Denormalizes properties from the MetaInfo anchor.
/// </summary>
public class IdentityValueResponseDto
{
    public Guid Id { get; set; }
    public Guid MetaInfoId { get; set; }

    // Denormalized Identity Properties
    public string Title { get; set; } = "";
    public string Slug { get; set;} = "";
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }

    // Domain Properties
    public Guid IdentityDefinitionId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public bool IsDefault { get; set; }
}

/// <summary>
/// List response for identity values.
/// </summary>
public class IdentityValueListResponseDto
{
    public IEnumerable<IdentityValueResponseDto> Items { get; set; } = Enumerable.Empty<IdentityValueResponseDto>();
    public int TotalCount { get; set; }
}