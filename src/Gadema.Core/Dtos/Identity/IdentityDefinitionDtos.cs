// =============================================================================
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Enums;

namespace Gadema.Core.Dtos.Identity;

/// <summary>
/// DTO for creating an identity definition (race, faction, alignment, etc.).
/// </summary>
public class IdentityDefinitionUpdateDto
{
    /// <summary>
    /// ID of the project this definition belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Type of identity (Race, Faction, Alignment, Guild).
    /// </summary>
    [EnumDataType(typeof(IdentityTypeEnum)), Required]
    public IdentityTypeEnum IdentityType { get; set; }

    /// <summary>
    /// Display name for the identity type.
    /// </summary>
    [Required, MaxLength(128)]
    public string IdentityTypeName { get; set; } = "";

    /// <summary>
    /// Whether this identity is required for characters.
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// Default value for this identity type.
    /// </summary>
    [MaxLength(256)]
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Description of this identity type.
    /// </summary>
    [MaxLength(1024)]
    public string? Description { get; set; }
}

/// <summary>
/// Response DTO for an identity definition.
/// </summary>
public class IdentityDefinitionResponseDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public int IdentityType { get; set; }
    public string IdentityTypeName { get; set; } = "";
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// List response for identity definitions.
/// </summary>
public class IdentityDefinitionListResponseDto
{
    public IEnumerable<IdentityDefinitionResponseDto> Items { get; set; } = Enumerable.Empty<IdentityDefinitionResponseDto>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
