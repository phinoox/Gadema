// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Attributes;

/// <summary>
/// DTO for character attributes storage.
/// </summary>
public class CharacterAttributesResponseDto
{
    public Guid MetaInfoId { get; set; }
    public string MetaInfoName { get; set; } = "";
    public Guid AttributeDefinitionId { get; set; }
    public string AttributeName { get; set; } = "";
    public decimal? CurrentValue { get; set; }
    public bool CalculatedFromTemplate { get; set; }
    public bool OverridesFormula { get; set; }
}

/// <summary>
/// List response for character attributes.
/// </summary>
public class CharacterAttributesListResponseDto
{
    public IEnumerable<CharacterAttributesResponseDto> Items { get; set; } = Enumerable.Empty<CharacterAttributesResponseDto>();
    public int TotalCount { get; set; }
}
