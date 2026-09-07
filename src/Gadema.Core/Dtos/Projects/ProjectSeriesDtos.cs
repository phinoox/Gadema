// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Projects;

/// <summary>
/// DTO for creating a project series.
/// </summary>
public class ProjectSeriesCreateDto
{
    /// <summary>
    /// Title of the series.
    /// </summary>
    [Required, MaxLength(128)]
    public string Title { get; set; } = "";

    /// <summary>
    /// URL-friendly slug.
    /// </summary>
    [MaxLength(128)]
    public string? Slug { get; set; }

    /// <summary>
    /// Series-wide description.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
}

/// <summary>
/// DTO for updating a project series.
/// </summary>
public class ProjectSeriesUpdateDto
{
    /// <summary>
    /// Title of the series.
    /// </summary>
    [MaxLength(128)]
    public string? Title { get; set; }

    /// <summary>
    /// URL-friendly slug.
    /// </summary>
    [MaxLength(128)]
    public string? Slug { get; set; }

    /// <summary>
    /// Series-wide description.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
}

/// <summary>
/// Response DTO for a project series.
/// </summary>
public class ProjectSeriesResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Slug { get; set; } = "";
    public string? Description { get; set; }
}

/// <summary>
/// List response for project series.
/// </summary>
public class ProjectSeriesListResponseDto
{
    public IEnumerable<ProjectSeriesResponseDto> Items { get; set; } = Enumerable.Empty<ProjectSeriesResponseDto>();
    public int TotalCount { get; set; }
}
