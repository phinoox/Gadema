using Gadema.Core.Models.Base.Projects;
using Gadema.Core.Models.Writing.Characters;

namespace Gadema.Core.Models.Writing.WorldBuilding;

    /// <summary>
/// Represents a geographic location within the game world.
/// Locations can be organized hierarchically (e.g., Country → Region → City) and track character presence.
    /// </summary>
[ModelDependency(typeof(Project), typeof(ContentMetaInfo))]
public class WorldLocation
{
    /// <summary>
    /// Unique identifier for the world location.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required] public Guid MetaInfoId { get; set; }

    /// <summary>
    /// Navigation property for the location's identity anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    /// <summary>
    /// The classification of this location (e.g., Country, City, Landmark).
    /// </summary>
    [Required] public LocationType LocationType { get; set; }

    /// <summary>
    /// The ID of the parent location in the hierarchy.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Navigation property for the parent location.
    /// </summary>
    [ForeignKey("ParentId")]
    public virtual WorldLocation? Parent { get; set; }

    /// <summary>
    /// Collection of child locations contained within this one.
    /// </summary>
    public virtual ICollection<WorldLocation> Children { get; set; } = new List<WorldLocation>();

    /// <summary>
    /// A description of the location's geography, climate, or significance.
    /// </summary>
    [MaxLength(4096)] public string? Description { get; set; }
    
    /// <summary>
    /// Collection of character states that reference this location (current or historical presence).
    /// </summary>
    public virtual ICollection<CharacterState> CharacterStates { get; set; } = new List<CharacterState>();
}

/// <summary>
/// Defines the hierarchical classification levels for world locations.
/// </summary>
public enum LocationType
{
    /// <summary>A large-scale political or geographic entity.</summary>
    Country = 0,
    /// <summary>A sub-division of a country.</summary>
    Region = 1,
    /// <summary>A populated urban center.</summary>
    City = 2,
    /// <summary>A small settlement or community.</summary>
    Village = 3,
    /// <summary>A landmark site or point of interest.</summary>
    Landmark = 4,
    ///<summary>A user-defined location type.</summary>
    Custom = 5
}