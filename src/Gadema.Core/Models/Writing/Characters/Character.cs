namespace Gadema.Core.Models.Writing.Characters;

/// <summary>
/// Represents the primary entity for a character within the narrative structure.
/// Acts as an aggregator that links identity (via <see cref="ContentMetaInfo"/>),
/// static backstory (via <see cref="CharacterStoryProfile"/>), and dynamic story progression (via <see cref="CharacterState"/>).
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))]
public class Character
{
    /// <summary>
    /// Unique identifier for the character.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required] public Guid MetaInfoId { get; set; }
    
    /// <summary>
    /// Navigation property for the character's identity anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    /// <summary>
    /// The full legal or formal name of the character.
    /// </summary>
    [Required, MaxLength(128)]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// An optional alias, moniker, or commonly used name for the character.
    /// </summary>
    [MaxLength(256)]
    public string? NickName { get; set; }

    /// <summary>
    /// The ID of the character's static story profile (backstory, traits).
    /// </summary>
    public Guid? StoryProfileId { get; set; }
    
    /// <summary>
    /// Navigation property for the character's permanent backstory and personality details.
    /// </summary>
    [ForeignKey("StoryProfileId")]
    public virtual CharacterStoryProfile? StoryProfile { get; set; }
    
    /// <summary>
    /// The ID of the character's most recent dynamic state in the narrative.
    /// </summary>
    public Guid? CurrentStateId { get; set; }
    
    /// <summary>
    /// Navigation property for the character's current status and attributes within the story.
    /// </summary>
    [ForeignKey("CurrentStateId")]
    public virtual CharacterState? CurrentState { get; set; }
    
    /// <summary>
    /// A collection of all historical states representing the character's evolution throughout the narrative.
    /// </summary>
    public virtual ICollection<CharacterState> States { get; set; } = new List<CharacterState>();
}

