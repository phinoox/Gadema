// =============================================================================

using Gadema.Core.Enums;
using Gadema.Core.Models.Characters;
using Gadema.Core.Models.Narrative;
using Gadema.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Writing;

/// <summary>
/// Represents a Scene - the "Unit of Work" for writing in GaDeMa.
/// Contains RawText (Markdown) and links to MetaInfo, StoryBeats, and SceneSegments.
/// This is where the writer enters "Flow State" with zero friction.
/// </summary>
[ModelDependency(typeof(Project), typeof(StoryOutline))]
public class Scene
{
    /// <summary>
    /// Unique identifier for this scene.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    // ========================================================================
    // MetaInfo Link (The "Metadata Wrapper")
    // ========================================================================
    
    /// <summary>
    /// FK to the MetaInfo (MetaInfo) that holds Title, Slug, Tags for this scene.
    /// The MetaInfo acts as the source of truth for entity metadata.
    /// </summary>
    [Required]
    public Guid MetaInfoId { get; set; }

    // Navigation property: MetaInfo (MetaInfo)
    [ForeignKey("MetaInfoId")]
    public virtual MetaInfo MetaInfo { get; set; } = null!;

    public int? OrderIndex { get; set; }

     [Required] public Guid StoryChapterId { get; set; }
    [ForeignKey("StoryChapterId")] public virtual StoryChapter StoryChapter { get; set; } = null!;

    // ========================================================================
    // RawText (The "Canvas")
    // ========================================================================
    
    /// <summary>
    /// The raw Markdown text of the scene. This is the primary content field.
    /// Users write here with zero friction - tags, links, and markers are parsed dynamically.
    /// </summary>
    [Required]
    public string RawText { get; set; } = "";

    // ========================================================================
    // Narrative Structure Links
    // ========================================================================
    
    // ========================================================================
    // SceneSegments (The "Markers")
    // ========================================================================
    
    /// <summary>
    /// Collection of SceneSegments (unique token markers) within this scene.
    /// Segments index interactive elements like [dialog:123] without storing text indices.
    /// </summary>
    public virtual ICollection<SceneSegment> SceneSegments { get; set; } = new List<SceneSegment>();

    // ========================================================================
    // StoryBeats (The "Landmarks")
    // ========================================================================
    
    /// <summary>
    /// Collection of StoryBeats linked to this scene via the junction table.
    /// A scene can fulfill multiple narrative beats (Many-to-Many).
    /// </summary>
    public virtual ICollection<StoryBeat> StoryBeats { get; set; } = new List<StoryBeat>();

    // ========================================================================
    // Character Relations
    // ========================================================================
    
    /// <summary>
    /// Collection of character relations triggered in this scene.
    /// Used for the "Character Relations" graph in the Right Panel.
    /// </summary>
    public virtual ICollection<CharacterRelation> CharacterRelations { get; set; } = new List<CharacterRelation>();
    
    /// <summary>
    /// Collection of character states established or changed in this scene.
    /// Links back to CharacterState.TriggerSceneId for event-driven tracking.
    /// </summary>
    public virtual ICollection<Models.Characters.CharacterState> CharacterStates { get; set; } = new List<Models.Characters.CharacterState>();
    
    // ========================================================================
    // Versioning & History
    // ========================================================================
    
    /// <summary>
    /// Collection of ContentSnapshots (milestones) for this scene.
    /// Backs the "Save Ritual" timeline in the Top Panel.
    /// </summary>
    public virtual ICollection<ContentSnapshot> Snapshots { get; set; } = new List<ContentSnapshot>();
    
}
