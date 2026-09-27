/// <summary>
/// Data transfer object for creating a new identity definition (e.g., "Species", "Eye Color").
/// </summary>
public class IdentityDefinitionCreateDto
{
    /// <summary>
    /// The metadata required to establish the definition's identity.
    /// </summary>
    [Required]
    public BaseMetaInfoCreateData ContentMetaInfo { get; set; } = new();

    /// <summary>
    /// The data type used for values associated with this definition (e.g., String, Integer).
    /// </summary>
    [Required]
    public IdentityDataTypeEnum DataType { get; set; }

    /// <summary>
    /// The display name of the identity type (e.g., "Species").
    /// </summary>
    [Required, MaxLength(128)]
    public string Name { get; set; } = "";

    /// <summary>
    /// A description of what this identity definition represents.
    /// </summary>
    [MaxLength(256)]
    public string? Description { get; set; }

    /// <summary>
    /// Indicates if this attribute is mandatory for a character/entity to have.
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// The unique identifier of the project this definition belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing identity definition.
/// </summary>
public class IdentityDefinitionUpdateDto
{
    /// <summary>
    /// The identity payload used by the sync strategy to update meta-information (e.g., Title, Slug).
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    /// <summary>
    /// The updated data type for this definition.
    /// </summary>
    public IdentityDataTypeEnum? DataType { get; set; }

    /// <summary>
    /// The updated display name of the definition.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The updated description of the definition.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether to update the requirement status.
    /// </summary>
    public bool? IsRequired { get; set; }

    /// <summary>
    /// Whether to update the active status of this definition.
    /// </summary>
    public bool? IsActive { get; set; }
}

/// <summary>
/// Represents an identity definition, including its type and metadata.
/// </summary>
public class IdentityDefinitionResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// The data type used for values associated with this definition.
    /// </summary>
    public IdentityDataTypeEnum DataType { get; set; }

    /// <summary>
    /// The display name of the identity type.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// A description of the definition.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates if this attribute is mandatory.
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// The unique identifier of the project this definition belongs to.
    /// </summary>
    public Guid ProjectId { get; set; }
}

/// <summary>
/// A collection of identity definitions, typically used for paginated lists.
/// </summary>
public class IdentityDefinitionListResponseDto
{
    /// <summary>
    /// The list of retrieved identity definitions.
    /// </summary>
    public IEnumerable<IdentityDefinitionResponseDto> Items { get; set; } = Enumerable.Empty<IdentityDefinitionResponseDto>();

    /// <summary>
    /// Total number of definitions found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}