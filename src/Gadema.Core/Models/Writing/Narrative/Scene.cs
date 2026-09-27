using Gadema.Core.Models.Base.Infrastructure;
using Gadema.Core.Models.Base.Projects;
using Gadema.Core.Models.Writing.Characters;

namespace Gadema.Core.Models.Writing.Narrative;

/// <summary>
/// Represents a Scene - the fundamental "unit of work" for narrative writing.
/// A scene contains raw markdown text and links to structural elements like
/// story chapters, beats, segments, and evolving character states.
/// </summary>
[ModelDependency(typeof(Project), typeof(StoryOutline))]
public class Scene
{
    /// <summary>
    /// Unique identifier for this scene.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    // ========================================================================
    // ContentMetaInfo Link (The "Metadata Wrapper")
    // ========================================================================
    
    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required]
    public Guid MetaInfoId { get; set; }

    /// <summary>
    /// Navigation property for the character's identity anchor.
    /// </summary>
    // Navigation property: ContentMetaInfo (ContentMetaInfo)
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;
    /// <summary>
    /// The sort order of this scene within its parent chapter.
    /// </summary>
    public int? OrderIndex { get; set; }
    /// <summary>
    /// The ID of the story chapter this scene belongs to.
        [Required] public Guid StoryChapterId { get; set; }
    /// <summary>
    /// Navigation property for the parent story chapter.
    /// </summary>
    [ForeignKey("StoryChapterId")] public virtual StoryChapter StoryChapter { get; set; } = null!;
    // ========================================================================
    // RawText (The "Canvas")
    // ========================================================================
    
    /// <summary>
    /// The raw Markdown text of the scene. This is the primary content field
    /// where writers enter their narrative, utilizing dynamic markers for interactivity.
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
    /// Collection of <see cref="SceneSegment"/> entities that act as markers within this scene.
    /// Segments index interactive elements like dialogue or character actions without
    /// relying on fragile text indices.
    /// </summary>
    public virtual ICollection<SceneSegment> SceneSegments { get; set; } = new List<SceneSegment>();

    // ========================================================================
    // StoryBeats (The "Landmarks")
    // ========================================================================
    
    /// <summary>
    /// Collection of <see cref="StoryBeat"/> entities linked to this scene.
    /// A single scene can fulfill multiple narrative beats in a many-to-many relationship.
    /// </summary>
    public virtual ICollection<StoryBeat> StoryBeats { get; set; } = new List<StoryBeat>();

    // ========================================================================
    // Character Relations
    // ========================================================================
    
    /// <summary>
    /// Collection of character relations that are established or modified in this scene.
    /// </summary>
    public virtual ICollection<CharacterRelation> CharacterRelations { get; set; } = new List<CharacterRelation>();
    
    /// <summary>
    /// Collection of character states (e.g., location, status) changed within this scene.
    /// </summary>
    public virtual ICollection<Characters.CharacterState> CharacterStates { get; set; } = new List<Characters.CharacterState>();
    
    // ========================================================================
    // Versioning & History
    // ========================================================================
    
    /// <summary>
    /// Collection of <see cref="ContentSnapshot"/> milestones for this scene,
    /// supporting the "Save Ritual" versioning system.
    /// </summary>
    public virtual ICollection<ContentSnapshot> Snapshots { get; set; } = new List<ContentSnapshot>();

}

