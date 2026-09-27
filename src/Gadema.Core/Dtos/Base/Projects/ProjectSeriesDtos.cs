/// <summary>
/// Data transfer object for creating a new project series.
/// </summary>
public class ProjectSeriesCreateDto
{
    /// <summary>
    /// The identity and metadata for the new series.
    /// </summary>
    [Required] 
    public ProjectSeriesMetaInfoCreateData MetaInfo { get; set; } = new();

    /// <summary>
    /// A high-level description of the series.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing project series.
/// </summary>
public class ProjectSeriesUpdateDto
{
    /// <summary>
    /// The identity payload used by the Identity Sync Strategy to update meta-info.
    /// </summary>
    public ProjectSeriesMetaInfoUpdateData? MetaInfo { get; set; }

    /// <summary>
    /// The updated description of the series.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
}

// --- Data Payloads (The "What") ---

/// <summary>
/// Metadata required to initialize a new project series.
/// </summary>
public class ProjectSeriesMetaInfoCreateData : BaseMetaInfoCreateData
{
    
}

/// <summary>
/// Payload for updating the meta-information of a project series.
/// </summary>
public class ProjectSeriesMetaInfoUpdateData : BaseMetaInfoUpdateData
{
 
}

// --- Response DTOs (The "Contract") ---

/// <summary>
/// Represents a project series' core details for client consumption.
/// </summary>
public class ProjectSeriesResponseDto
{
    /// <summary>
    /// The unique identifier of the series.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The title of the series.
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// The URL-friendly slug of the series.
    /// </summary>
    public string Slug { get; set; } = "";

    /// <summary>
    /// A high-level description of the series.
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// A collection of project series responses.
/// </summary>
public class ProjectSeriesListResponseDto
{
    /// <summary>
    /// The list of retrieved series.
    /// </summary>
    public IEnumerable<ProjectSeriesResponseDto> Items { get; set; } = Enumerable.Empty<ProjectSeriesResponseDto>();

    /// <summary>
    /// Total number of records available across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}