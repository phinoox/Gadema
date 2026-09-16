// =============================================================================
namespace Gadema.Core.Dtos.Identity;

/// <summary>
/// Response DTO for character identity assignment.
/// </summary>
public class CharacterIdentityResponseDto
{
    public Guid Id { get; set; }
    public Guid MetaInfoId { get; set; }
    public Guid? IdentityDefinitionId { get; set; }
    public string? IdentityTypeName { get; set; }
    public Guid? IdentityValueId { get; set; }
    public string? ValueName { get; set; }
    public string? DisplayText { get; set; }
    public int IdentityType { get; set; }
    public bool IsPrimary { get; set; }
}

/// <summary>
/// List response for character identities.
/// </summary>
public class CharacterIdentityListResponseDto
{
    public IEnumerable<CharacterIdentityResponseDto> Items { get; set; } = Enumerable.Empty<CharacterIdentityResponseDto>();
    public int TotalCount { get; set; }
}
