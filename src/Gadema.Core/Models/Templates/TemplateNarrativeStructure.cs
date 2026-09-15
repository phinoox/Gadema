// =============================================================================
// TemplateNarrativeStructure - Entity for narrative structure definitions in project templates
// =============================================================================

using Gadema.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gadema.Core.Models.Templates;

/// <summary>
/// Definition of narrative structures (act sequences, plot points) for project templates.
/// Provides pre-configured story arcs and sequence arrangements for game templates.
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class TemplateNarrativeStructure
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Project Template ID")]
    public Guid ProjectTemplateId { get; set; }

    // Navigation property for ProjectTemplate (Many-to-One)
    [ForeignKey("ProjectTemplateId")]
    public virtual ProjectTemplate ProjectTemplate { get; set; }
    
    [Required, MaxLength(128), Display(Name = "Sequence Name")]
    public string SequenceName { get; set; } = "";
    
    public int OrderIndex { get; set; } = 0;
    
    public bool IsDefaultStructure { get; set; } = true;
    
    [MaxLength(2048)]
    public string? Description { get; set; }

}
