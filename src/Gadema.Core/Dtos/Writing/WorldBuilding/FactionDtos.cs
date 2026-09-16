using System.ComponentModel.DataAnnotations;
using Gadema.Core.Models.Base.MetaInfo;

namespace Gadema.Core.Dtos.Writing.WorldBuilding;

/// <summary>
/// DTO for creating a faction.
/// </summary>
public class FactionCreateDto
{
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();
    
    /// <summary>
    /// The faction's ideology, mission, or core purpose.
    /// </summary>
    [MaxLength(4096)] public string? Ideology { get; set; }

    /// <summary>
    /// The faction's goals or objectives.
    /// </summary>
    [MaxLength(4096)] public string? Goals { get; set; }

    /// <summary>
    /// Optional reference to the faction's primary location.
    /// </summary>
    public Guid? LocationId { get; set; }
}

/// <summary>
/// DTO for updating a faction.
/// </summary>
public class FactionUpdateDto : UpdateRequestDto
{
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    [MaxLength(4096)] public string? Ideology { get; set; }
    [MaxLength(4096)] public string? Goals { get; set; }
    public Guid? LocationId { get; set; }
}

/// <summary>
/// Response DTO for a faction. Inherits ContentMetaInfo state.
/// </summary>
public class FactionResponseDto : MetaInfoResponseBaseDto
{
    public string? Ideology { get; set; }
    public string? Goals { get; set; }
    public Guid? LocationId { get; set; }
    public string? LocationName { get; set; }  // Denormalized for convenience
}

/// <summary>
/// List response for factions.
/// </summary>
public class FactionListResponseDto
{
    public IEnumerable<FactionResponseDto> Items { get; set; } = Enumerable.Empty<FactionResponseDto>();
    public int TotalCount { get; set; }
}