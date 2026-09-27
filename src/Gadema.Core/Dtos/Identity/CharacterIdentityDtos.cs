/// <summary>
/// Represents a character's specific identity attribute (e.g., "Eye Color: Blue" or "Species: Human").
/// </summary>
public class CharacterIdentityResponseDto
{
    /// <summary>
    /// The unique identifier of the identity record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the associated meta-information anchor.
    /// </summary>
    public Guid MetaInfoId { get; set; }

    /// <summary>
    /// The unique identifier of the identity definition (e.g., "Species" or "Eye Color").
    /// </summary>
    public Guid? IdentityDefinitionId { get; set; }

    /// <summary>
    /// The display name of the identity type for UI convenience.
    /// </summary>
    public string? IdentityTypeName { get; set; }

    /// <summary>
    /// The unique identifier of the specific identity value (e.s. "Human", "Blue").
    /// </summary>
    public Guid? IdentityValueId { get; set; }

    /// <summary>
    /// The display name of the identity value for UI convenience.
    /// </summary>
    public string? ValueName { get; set; }

    /// <summary>
    /// A pre-formatted string combining type and value for easy display (e.g., "Species: Human").
    /// </summary>
    public string? DisplayText { get; set; }

    /// <summary>
    /// The integer representation of the identity type.
    /// </summary>
    public int IdentityType { get; set; }

    /// <summary>
    /// Indicates if this is the character's primary identity attribute.
    /// </summary>
    public bool IsPrimary { get; set; }
}

/// <summary>
/// A collection of character identities, typically used for paginated lists.
/// </summary>
public class CharacterIdentityListResponseDto
{
    /// <summary>
    /// The list of retrieved character identities.
    /// </summary>
    public IEnumerable<CharacterIdentityResponseDto> Items { get; set; } = Enumerable.Empty<CharacterIdentityResponseDto>();

    /// <summary>
    /// Total number of identity records found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}
