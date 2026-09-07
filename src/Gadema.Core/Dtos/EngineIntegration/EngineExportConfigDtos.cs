// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.EngineIntegration;

/// <summary>
/// DTO for creating an engine export configuration.
/// </summary>
public class EngineExportConfigCreateDto
{
    /// <summary>
    /// ID of the project this config belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Target engine type (Unity, Unreal, Both).
    /// </summary>
    public int EngineType { get; set; }

    /// <summary>
    /// Export format.
    /// </summary>
    [Required]
    public string ExportFormat { get; set; } = "";

    /// <summary>
    /// Whether this is the default config for this project.
    /// </summary>
    public bool IsDefaultConfig { get; set; } = true;

    /// <summary>
    /// JSON for custom field mappings.
    /// </summary>
    [MaxLength(4096)]
    public string? FieldMappingsJson { get; set; }

    /// <summary>
    /// Description of this configuration.
    /// </summary>
    [MaxLength(1024)]
    public string? Description { get; set; }
}

/// <summary>
/// Response DTO for an engine export configuration.
/// </summary>
public class EngineExportConfigResponseDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public int EngineType { get; set; }
    public string ExportFormat { get; set; } = "";
    public bool IsDefaultConfig { get; set; }
    public string? FieldMappingsJson { get; set; }
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }
}

/// <summary>
/// List response for engine export configurations.
/// </summary>
public class EngineExportConfigListResponseDto
{
    public IEnumerable<EngineExportConfigResponseDto> Items { get; set; } = Enumerable.Empty<EngineExportConfigResponseDto>();
    public int TotalCount { get; set; }
}
