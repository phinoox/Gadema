using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Models.Writing.WorldBuilding;

namespace Gadema.Core.Dtos.Writing.WorldBuilding;

/// <summary>
/// Data transfer object for creating a new world location (e.g., Country, City).
/// </summary>
public class WorldLocationCreateDto
{
    /// <summary>
    /// The metadata required to establish the location's identity.
    /// </summary>
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();
    
    /// <summary>
    /// The type of the location (e.g., Country, Region, City).
    /// </summary>
    [Required] public LocationType LocationType { get; set; }
    /// <summary>
    /// The unique identifier of the parent location. Null for top-level locations.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// A description of the location's significance or features.
    /// </summary>
    [MaxLength(4096)] public string? Description { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing world location.
/// </summary>
public class WorldLocationUpdateDto : UpdateRequestDto
{
/// <summary>
    /// The identity payload used by the sync strategy to update meta-information.
/// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    /// <summary>
    /// The updated type of the location.
    /// </summary>
    public LocationType? LocationType { get; set; }

    /// <summary>
    /// The updated parent location identifier.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// The updated description of the location.
    /// </summary>
    [MaxLength(4096)] public string? Description { get; set; }
}

/// <summary>
/// Represents a world location, including its type and hierarchical position.
/// </summary>
public class WorldLocationResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// The type of the location.
    /// </summary>
    public LocationType LocationType { get; set; }

    /// <summary>
    /// The unique identifier of the parent location.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// A description of the location's significance.
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// A collection of world locations, typically used for paginated lists.
/// </summary>
public class WorldLocationListResponseDto
{
    /// <summary>
    /// The list of retrieved world locations.
    /// </summary>
    public IEnumerable<WorldLocationResponseDto> Items { get; set; } = Enumerable.Empty<WorldLocationResponseDto>();

    /// <summary>
    /// Total number of locations found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}
