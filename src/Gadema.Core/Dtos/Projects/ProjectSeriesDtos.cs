// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Projects;


/// <summary>
/// DTO for creating a project series.
/// </summary>
public class ProjectSeriesCreateDto
{
    /// <summary>
    /// The identity data for the series.
    /// </summary>
    [Required] 
    public ProjectSeriesMetaInfoCreateData ContentMetaInfo { get; set; } = new();

    [MaxLength(4096)]
    public string? Description { get; set; }
}

/// <summary>
/// DTO for updating a project series.
/// </summary>
public class ProjectSeriesUpdateDto
{
    /// <summary>
    /// The identity data payload used by the Identity Sync Strategy.
    /// </summary>
    public ProjectSeriesMetaInfoUpdateData? ContentMetaInfo { get; set; }

    [MaxLength(4096)]
    public string? Description { get; set; }
}

// --- Data Payloads (The "What") ---

public class ProjectSeriesMetaInfoCreateData : BaseMetaInfoCreateData
{
    
}

public class ProjectSeriesMetaInfoUpdateData : BaseMetaInfoUpdateData
{
 
}

// --- Response DTOs (The "Contract") ---

public class ProjectSeriesResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Slug { get; set; } = "";
    public string? Description { get; set; }
}

public class ProjectSeriesListResponseDto
{
    public IEnumerable<ProjectSeriesResponseDto> Items { get; set; } = Enumerable.Empty<ProjectSeriesResponseDto>();
    public int TotalCount { get; set; }
}