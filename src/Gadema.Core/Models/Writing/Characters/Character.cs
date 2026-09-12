// src/Gadema.Core/Models/Characters/Character.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Gadema.Core.Models;

namespace Gadema.Core.Models.Characters;

/// <summary>
/// Entry point for all character information. Links MetaInfo (identity/searching) with:
/// - CharacterStoryProfile (static backstory/traits, one-to-one)
/// - CharacterState (dynamic story state, one-to-many)
/// This is the glue between story and game-related character data.
/// </summary>
[ModelDependency(typeof(MetaInfo), typeof(CharacterStoryProfile))]
public class Character
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Links to MetaInfo for the character's identity (title = character name, slug, description).
    /// Enables searching and filtering by character name.
    /// </summary>
    [Required] public Guid MetaInfoId { get; set; }
    
    [ForeignKey("MetaInfoId")]
    public virtual MetaInfo MetaInfo { get; set; } = null!;
    
    /// <summary>
    /// Static backstory and personality information (one-to-one).
    /// </summary>
    public Guid? StoryProfileId { get; set; }
    
    [ForeignKey("StoryProfileId")]
    public virtual CharacterStoryProfile? StoryProfile { get; set; }
    
    /// <summary>
    /// The character's current (latest) state. Can be null if no states have been created yet.
    /// </summary>
    public Guid? CurrentStateId { get; set; }
    
    [ForeignKey("CurrentStateId")]
    public virtual CharacterState? CurrentState { get; set; }
    
    /// <summary>
    /// Collection of all character states over time.
    /// Each state represents the character at a different point in the story (chapter/arc).
    /// </summary>
    public virtual ICollection<CharacterState> States { get; set; } = new List<CharacterState>();
}
