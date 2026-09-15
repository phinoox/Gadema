// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.WorldBuilding;

/// <summary>
/// DTO for creating a world location.
/// </summary>
public class WorldLocationCreateDto
{
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();
    
    /// <summary>
    /// Type of the location (Country, Region, City, etc.).
    /// </summary>
    [Required] public LocationType LocationType { get; set; }

    /// <summary>
    /// ID of the parent location. Null for top-level locations (e.g., countries).
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Description of the location's significance.
    /// </summary>
    [MaxLength(4096)] public string? Description { get; set; }
}

/// <summary>
/// DTO for updating a world location.
/// </summary>
public class WorldLocationUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// ContentMetaInfo fields (nullable — omit to keep current).
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    /// <summary>
    /// Type of the location.
    /// </summary>
    public LocationType? LocationType { get; set; }

    /// <summary>
    /// ID of the parent location. Null for top-level locations.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Description of the location's significance.
    /// </summary>
    [MaxLength(4096)] public string? Description { get; set; }
}

/// <summary>
/// Response DTO for a world location. Inherits ContentMetaInfo state (Id, MetaInfoId, MetaInfoTitle, Status, IsPublic, CreatedAt, LastModifiedAt).
/// </summary>
public class WorldLocationResponseDto : MetaInfoResponseBaseDto
{
    public LocationType LocationType { get; set; }
    public Guid? ParentId { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// List response for world locations.
/// </summary>
public class WorldLocationListResponseDto
{
    public IEnumerable<WorldLocationResponseDto> Items { get; set; } = Enumerable.Empty<WorldLocationResponseDto>();
    public int TotalCount { get; set; }
}