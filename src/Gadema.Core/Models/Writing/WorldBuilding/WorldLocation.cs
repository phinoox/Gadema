using Gadema.Core.Models.Base.Projects;
using Gadema.Core.Models.Writing.Characters;

namespace Gadema.Core.Models.Writing.WorldBuilding;

[ModelDependency(typeof(Project), typeof(ContentMetaInfo))]
public class WorldLocation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required] public Guid MetaInfoId { get; set; }
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    [Required] public LocationType LocationType { get; set; }

    public Guid? ParentId { get; set; }

    [ForeignKey("ParentId")]
    public virtual WorldLocation? Parent { get; set; }

    public virtual ICollection<WorldLocation> Children { get; set; } = new List<WorldLocation>();

    [MaxLength(4096)] public string? Description { get; set; }
    
    /// <summary>
    /// Collection of character states that reference this location (current or historical presence).
    /// </summary>
    public virtual ICollection<CharacterState> CharacterStates { get; set; } = new List<CharacterState>();
}

public enum LocationType
{
    Country = 0,
    Region = 1,
    City = 2,
    Village = 3,
    Landmark = 4,
    Custom = 5
}