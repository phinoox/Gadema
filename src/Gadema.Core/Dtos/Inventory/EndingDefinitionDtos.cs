// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Inventory;

/// <summary>
/// DTO for creating an ending definition.
/// </summary>
public class EndingDefinitionCreateDto
{
    /// <summary>
    /// ID of the project this ending belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Title of the ending.
    /// </summary>
    [Required, MaxLength(128)]
    public string Title { get; set; } = "";

    /// <summary>
    /// URL-friendly slug.
    /// </summary>
    [MaxLength(128)]
    public string? Slug { get; set; }

    /// <summary>
    /// Description of the ending.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }

    /// <summary>
    /// Conditions JSON for triggering this ending.
    /// </summary>
    [MaxLength(1024)]
    public string? ConditionsJson { get; set; }

    /// <summary>
    /// Indicates if the ending is published.
    /// </summary>
    public bool Published { get; set; } = false;
}

/// <summary>
/// Response DTO for an ending definition.
/// </summary>
public class EndingDefinitionResponseDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = "";
    public string Slug { get; set; } = "";
    public string? Description { get; set; }
    public string? ConditionsJson { get; set; }
    public bool Published { get; set; }
}

/// <summary>
/// List response for ending definitions.
/// </summary>
public class EndingDefinitionListResponseDto
{
    public IEnumerable<EndingDefinitionResponseDto> Items { get; set; } = Enumerable.Empty<EndingDefinitionResponseDto>();
    public int TotalCount { get; set; }
}
