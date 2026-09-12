// src/Gadema.Core/Models/WorldBuilding/Faction.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gadema.Core.Models.Characters;
using Gadema.Core.Models.Projects;

namespace Gadema.Core.Models.WorldBuilding;

/// <summary>
/// Represents a faction, organization, or group within the game world.
/// Has ideology, goals, and optional location reference.
/// </summary>
[ModelDependency(typeof(Project), typeof(MetaInfo), typeof(WorldLocation))]
public class Faction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required] public Guid MetaInfoId { get; set; }
    [ForeignKey("MetaInfoId")]
    public virtual MetaInfo MetaInfo { get; set; } = null!;

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
    /// A faction can be headquartered in or associated with a world location.
    /// </summary>
    public Guid? LocationId { get; set; }

    [ForeignKey("LocationId")]
    public virtual WorldLocation? Location { get; set; }
    
    /// <summary>
    /// Collection of character states that reference this faction (current or historical affiliations).
    /// </summary>
    public virtual ICollection<CharacterState> CharacterStates { get; set; } = new List<CharacterState>();

    // Metadata
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
}