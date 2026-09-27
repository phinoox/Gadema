// =============================================================================
// TemplateNarrativeStructure - Entity for narrative structure definitions in project templates
// =============================================================================

namespace Gadema.Core.Models.Templates;

/// <summary>
/// Defines a pre-configured narrative structure (e.g., Act Sequences, Plot Points) for a project template.
/// This allows templates to provide ready-made story arcs and sequence arrangements for new projects.
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class TemplateNarrativeStructure
{
    /// <summary>
    /// Unique identifier for the template narrative structure.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// The ID of the project template this structure belongs to.
    /// </summary>
    [Required, Display(Name = "Project Template ID")]
    public Guid ProjectTemplateId { get; set; }

    /// <summary>
    /// Navigation property for the parent project template.
    /// </summary>
    // Navigation property for ProjectTemplate (Many-to-One)
    [ForeignKey("ProjectTemplateId")]
    public virtual ProjectTemplate ProjectTemplate { get; set; }
    
    /// <summary>
    /// The human-readable name of the sequence (e.g., "Act 1: Introduction", "The Rising Action").
    /// </summary>
    [Required, MaxLength(128), Display(Name = "Sequence Name")]
    public string SequenceName { get; set; } = "";
    
    /// <summary>
    /// The sort order for this sequence within the narrative structure.
    /// </summary>
    public int OrderIndex { get; set; } = 0;
    
    /// <summary>
    /// Indicates if this is the primary or default structure for the template.
    /// </summary>
    public bool IsDefaultStructure { get; set; } = true;
    
    /// <summary>
    /// A detailed description of what this narrative sequence represents.
    /// </summary>
    [MaxLength(2048)]
    public string? Description { get; set; }

}

