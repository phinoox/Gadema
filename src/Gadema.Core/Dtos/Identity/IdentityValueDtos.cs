using Gadema.Core.Dtos.Base.Infrastructure;

namespace Gadema.Core.Dtos.Identity;

/// <summary>
/// Data transfer object for creating a new identity value (e.g., "Human", "Blue").
/// </summary>
public class IdentityValueCreateDto
{
    /// <summary>
    /// The metadata required to establish the value's identity.
    /// </summary>
    [Required]
    public BaseMetaInfoCreateData ContentMetaInfo { get; set; } = new();

    /// <summary>
    /// The unique identifier of the parent IdentityDefinition this value belongs to.
    /// </summary>
    [Required]
    public Guid IdentityDefinitionId { get; set; }

    /// <summary>
    /// The display name of the identity value (e.g., "Human").
    /// </summary>
    [Required, MaxLength(128)] 
    public string Name { get; set; } = "";

    /// <summary>
    /// A description or context for this specific value.
    /// </summary>
    [MaxLength(4096)] 
    public string? Description { get; set; }

    /// <summary>
    /// The sort order index for this value within its definition.
    /// </summary>
    public int OrderIndex { get; set; } = 0;

    /// <summary>
    /// Indicates if this is the default value for the associated identity definition.
    /// </summary>
    public bool IsDefault { get; set; } = false;
}

/// <summary>
/// Data transfer object for updating an existing identity value.
/// </summary>
public class IdentityValueUpdateDto
{
    /// <summary>
    /// The identity payload used by the sync strategy to update meta-information.
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    /// <summary>
    /// The updated display name of the value.
    /// </summary>
    [MaxLength(128)] 
    public string? Name { get; set; }

    /// <summary>
    /// The updated description of the value.
    /// </summary>
    [MaxLength(4096)] 
    public string? Description { get; set; }

    /// <summary>
    /// The updated sort order index.
    /// </summary>
    public int? OrderIndex { get; set; }

    /// <summary>
    /// Whether to update the default status of this value.
    /// </summary>
    public bool? IsDefault { get; set; }
}

/// <summary>
/// Represents a specific identity value, including its metadata and association with a definition.
/// </summary>
public class IdentityValueResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// The unique identifier of the parent identity definition.
    /// </summary>
    public Guid IdentityDefinitionId { get; set; }

    /// <summary>
    /// The display name of the value.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// A description of the value.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The sort order index for this value.
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// Indicates if this is the default value for its definition.
    /// </summary>
    public bool IsDefault { get; set; }
}

/// <summary>
/// A collection of identity values, typically used for paginated lists.
/// </summary>
public class IdentityValueListResponseDto
{
    /// <summary>
    /// The list of retrieved identity values.
    /// </summary>
    public IEnumerable<IdentityValueResponseDto> Items { get; set; } = Enumerable.Empty<IdentityValueResponseDto>();

    /// <summary>
    /// Total number of values found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}