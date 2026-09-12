// =============================================================================
using Gadema.Core.Models.Narrative;
using Gadema.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Writing;

/// <summary>
/// Represents a story sequence (chapter) within a project.
/// Used for organizing narrative structure and beat sheets.
/// </summary>
[ModelDependency(typeof(Project))]
public class StoryChapter
{
    /// <summary>
    /// Unique identifier for the story sequence.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    //[Fixture(FixtureHintEnum.Omit)]
    [Required]
    public Guid MetaInfoId { get; set; }  
    
    [ForeignKey("MetaInfoId")]
    public virtual MetaInfo? MetaInfo { get; set; }

    [Required] public Guid StoryId { get; set; }
    [ForeignKey("StoryId")]
     public virtual Story Story { get; set; } = null!;

        
    /// <summary>
    /// Description of the sequence.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    
    /// <summary>
    /// Order index for sorting sequences.
    /// </summary>
    [Column("order_index"), Display(Name = "Order Index")]
    public int OrderIndex { get; set; } = 0;

      
    /// <summary>
    /// Navigation property: Collection of beats in this sequence.
    /// Foreign key: SequenceId (matches FK in StoryBeat)
    /// </summary>
    public virtual ICollection<Scene> Scenes { get; set; } = new List<Scene>();

}