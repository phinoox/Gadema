using Gadema.Core.Models.Base.MetaInfo;
using Gadema.Core.Models.Writing.Narrative;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gadema.Core.Models.Writing.Characters;

/// <summary>
/// Tracks evolving relationships between characters throughout the story.
/// This is an event-driven junction table that allows relationships to change over time.
/// </summary>
[ModelDependency(typeof(Character), typeof(Scene))]
public class CharacterRelation
{
    /// <summary>
    /// Unique identifier for this relationship record.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// ID of the source character (the one initiating or having the relationship).
    /// </summary>
    [Required]
    public Guid SourceCharacterId { get; set; }

    // Navigation property: Source Character
    [ForeignKey("SourceCharacterId")]
    public virtual Character SourceCharacter { get; set; } = null!;

    /// <summary>
    /// ID of the target character (the one being related to).
    /// </summary>
    [Required]
    public Guid TargetCharacterId { get; set; }

    // Navigation property: Target Character
    [ForeignKey("TargetCharacterId")]
    public virtual Character TargetCharacter { get; set; } = null!;

    /// <summary>
    /// Type of relationship (Ally, Enemy, Family, Romantic, etc.).
    /// </summary>
    [Required]
    public RelationTypeEnum RelationType { get; set; } = RelationTypeEnum.Alien;

    /// <summary>
    /// The scene where this relationship evolved or was first established.
    /// Allows the UI to show "They became allies in Scene 4."
    /// </summary>
    [Required]
    public Guid TriggerSceneId { get; set; }

    // Navigation property: Trigger Scene
    [ForeignKey("TriggerSceneId")]
    public virtual Scene TriggerScene { get; set; } = null!;

    /// <summary>
    /// Optional description of the relationship at this point in the story.
    /// </summary>
    [MaxLength(1024)]
    public string? Description { get; set; }

    /// <summary>
    /// Timestamp when this relationship record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Enum for character relationship types.
/// </summary>
public enum RelationTypeEnum
{
    /// <summary>No established relationship</summary>
    Alien = 0,
    
    /// <summary>Acquaintance or neutral contact</summary>
    Acquaintance = 1,
    
    /// <summary>Allied or cooperative</summary>
    Ally = 2,
    
    /// <summary>Hostile or opposing</summary>
    Enemy = 3,
    
    /// <summary>Blood relation or familial bond</summary>
    Family = 4,
    
    /// <summary>Romantic involvement</summary>
    Romantic = 5,
    
    /// <summary>Mentor-student or teacher-pupil</summary>
    Mentor = 6,
    
    /// <summary>Professional or work-related</summary>
    Professional = 7,
    
    /// <summary>Custom relationship type</summary>
    Custom = 8
}
