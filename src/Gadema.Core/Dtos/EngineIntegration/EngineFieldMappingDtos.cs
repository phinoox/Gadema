// =============================================================================
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Enums;

namespace Gadema.Core.Dtos.EngineIntegration;

/// <summary>
/// DTO for creating an engine field mapping.
/// </summary>
public class EngineFieldMappingCreateDto
{
    /// <summary>
    /// ID of the project this mapping belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Content type this mapping applies to.
    /// </summary>
    [EnumDataType(typeof(ContentTypeEnum)), Required]
    public ContentTypeEnum ContentType { get; set; }

    /// <summary>
    /// Source field name in the content model.
    /// </summary>
    [Required, MaxLength(128)]
    public string ContentFieldName { get; set; } = "";

    /// <summary>
    /// Target field name in the game engine.
    /// </summary>
    [MaxLength(256)]
    public string? EngineFieldName { get; set; }

    /// <summary>
    /// Whether this field is required for export.
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// Data type (Int, Float, String, Boolean).
    /// </summary>
    public int DataType { get; set; }

    /// <summary>
    /// Description of this mapping.
    /// </summary>
    [MaxLength(1024)]
    public string? Description { get; set; }

    /// <summary>
    /// Source column name from game engine.
    /// </summary>
    [MaxLength(256)]
    public string? SourceColumn { get; set; }

    /// <summary>
    /// Target column name in database.
    /// </summary>
    [MaxLength(256)]
    public string? TargetColumn { get; set; }
}

/// <summary>
/// Response DTO for an engine field mapping.
/// </summary>
public class EngineFieldMappingResponseDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public int ContentType { get; set; }
    public string ContentFieldName { get; set; } = "";
    public string? EngineFieldName { get; set; }
    public bool IsRequired { get; set; }
    public int DataType { get; set; }
    public string? Description { get; set; }
    public string? SourceColumn { get; set; }
    public string? TargetColumn { get; set; }
}

/// <summary>
/// List response for engine field mappings.
/// </summary>
public class EngineFieldMappingListResponseDto
{
    public IEnumerable<EngineFieldMappingResponseDto> Items { get; set; } = Enumerable.Empty<EngineFieldMappingResponseDto>();
    public int TotalCount { get; set; }
}
