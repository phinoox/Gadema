// =============================================================================
using Gadema.Core.Enums;
using Gadema.Core.Models.Narrative;
using Gadema.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Writing;

/// <summary>
/// Represents a Story Outline - the "Map" of the project's narrative intent.
/// Acts as a container for high-level ideas and StoryBeats (the "Landmarks").
/// This is a distinct document where users write summaries and scene prototypes.
/// Hierarchy: Project → StoryOutline → StoryBeat → Scene
/// </summary>
[ModelDependency(typeof(Project), typeof(StoryBeat))]
public class StoryOutline
{
    /// <summary>
    /// Unique identifier for the story outline.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public Guid MetaInfoId { get; set; }
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    [Required] public Guid StoryId { get; set; }
    [ForeignKey("StoryId")] 
    public virtual Story Story { get; set; } = null!;

    /// <summary>
    /// Summary of the outline section (legacy field, superseded by RawText).
    /// Kept for backward compatibility.
    /// </summary>
    [MaxLength(4096)]
    public string? Summary { get; set; } = "";

       
    // Replaced single RawText with dynamic, reorderable sections
    public virtual ICollection<OutlineSection> Sections { get; set; } = new List<OutlineSection>();

    
}
