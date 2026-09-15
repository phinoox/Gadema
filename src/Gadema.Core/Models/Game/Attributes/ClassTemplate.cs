// =============================================================================
using Gadema.Core.Models.Base.MetaInfo;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Game.Attributes;

/// <summary>
/// Class template definition for character scaling systems.
/// Used in project templates to define base class configurations.
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class ClassTemplate
{
    /// <summary>
    /// Unique identifier for the class template.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? MetaInfoId { get; set; }  
    
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo? ContentMetaInfo { get; set; }

    
    /// <summary>
    /// Name of the class (e.g., "Warrior", "Mage").
    /// </summary>
    [Required]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// Description of the class and its role.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    /// <summary>
    /// FK to the attribute set this template belongs to.
    /// </summary>
    [Required, Display(Name = "Attribute Set")]
    public Guid AttributeSetId { get; set; }

    // Navigation property for AttributeSet (Many-to-One)
    [ForeignKey("AttributeSetId")]
    public virtual AttributeSet AttributeSet { get; set; }
    
    /// <summary>
    /// Base level for all attributes in this class.
    /// </summary>
    public int BaseLevel { get; set; } = 1;
    
    /// <summary>
    /// Maximum level this class can reach (nullable).
    /// </summary>
    public int? MaxLevel { get; set; }

}