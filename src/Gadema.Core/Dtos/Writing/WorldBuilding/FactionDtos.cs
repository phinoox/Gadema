using Gadema.Core.Dtos.Base.Infrastructure;

namespace Gadema.Core.Dtos.Writing.WorldBuilding;

/// <summary>
/// Data transfer object for creating a new faction within the world.
/// </summary>
public class FactionCreateDto
{
    /// <summary>
    /// The metadata required to establish the faction's identity.
    /// </summary>
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();
    
    /// <summary>
    /// The core ideology, mission, or belief system of the faction.
    /// </summary>
    [MaxLength(4096)] public string? Ideology { get; set; }

    /// <summary>
    /// The primary objectives or goals pursued by the faction.
    /// </summary>
    [MaxLength(4096)] public string? Goals { get; set; }

    /// <summary>
    /// An optional reference to the faction's primary base of operations/location.
    /// </summary>
    public Guid? LocationId { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing faction.
/// </summary>
public class FactionUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// The identity payload used by the sync strategy to update meta-information.
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    /// <summary>
    /// The updated ideology of the faction.
    /// </summary>
    [MaxLength(4096)] public string? Ideology { get; set; }

    /// <summary>
    /// The updated goals or objectives.
    /// </summary>
    [MaxLength(4096)] public string? Goals { get; set; }

    /// <summary>
    /// The updated primary location identifier.
    /// </summary>
    public Guid? LocationId { get; set; }
}

/// <summary>
/// Represents a faction, including its ideology and goals.
/// </summary>
public class FactionResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// The core ideology or mission of the faction.
    /// </summary>
    public string? Ideology { get; set; }

    /// <summary>
    /// The primary goals or objectives of the faction.
    /// </summary>
    public string? Goals { get; set; }

    /// <summary>
    /// The unique identifier of the faction's location.
    /// </summary>
    public Guid? LocationId { get; set; }

    /// <summary>
    /// The display name of the associated location for UI convenience.
    /// </summary>
    public string? LocationName { get; set; }  // Denormalized for convenience
}

/// <summary>
/// A collection of factions, typically used for paginated lists.
/// </summary>
public class FactionListResponseDto
{
    /// <summary>
    /// The list of retrieved factions.
    /// </summary>
    public IEnumerable<FactionResponseDto> Items { get; set; } = Enumerable.Empty<FactionResponseDto>();

    /// <summary>
    /// Total number of factions found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}