using Gadema.Core.Models.Base.Projects;
using Gadema.Core.Models.Writing.Characters;

namespace Gadema.Core.Models.Writing.WorldBuilding;

/// <summary>
/// Represents a faction, organization, or group within the game world.
/// Factions have defined ideologies and goals, and may be anchored to specific locations.
/// </summary>
[ModelDependency(typeof(Project), typeof(ContentMetaInfo), typeof(WorldLocation))]
public class Faction
{
    /// <summary>
    /// Unique identifier for the faction.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required] public Guid MetaInfoId { get; set; }
    /// <summary>
    /// Navigation property for the faction's identity anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    /// <summary>
    /// The core ideology, mission statement, or driving philosophy of the faction.
    /// </summary>
    [MaxLength(4096)] public string? Ideology { get; set; }

    /// <summary>
    /// The primary objectives or long-term goals of the faction.
    /// </summary>
    [MaxLength(4096)] public string? Goals { get; set; }

    /// <summary>
    /// The ID of the world location where the faction is primarily based or headquartered.
    /// </summary>
    public Guid? LocationId { get; set; }

    /// <summary>
    /// Navigation property for the faction's primary geographic headquarters.
    /// </summary>
    [ForeignKey("LocationId")]
    public virtual WorldLocation? Location { get; set; }
    
    /// <summary>
    /// Collection of character states that track current or historical affiliations with this faction.
    /// </summary>
    public virtual ICollection<CharacterState> CharacterStates { get; set; } = new List<CharacterState>();

    /// <summary>
    /// The timestamp when the faction was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The timestamp of the last modification to this faction.
    /// </summary>
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
}