namespace Gadema.Core.Models.Game.Attributes;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a collection of attributes grouped together for a specific entity or template (e.g., "Core Stats", "Magic Attributes").
/// This allows for defining complete attribute systems tailored to different archetypes.
/// </summary>
[ModelDependency(typeof(Project), typeof(ContentMetaInfo))]
public class AttributeSet
{
    /// <summary>
    /// Unique identifier for the attribute set.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required]
    public Guid? MetaInfoId { get; set; }
    
    /// <summary>
    /// Navigation property for the attribute set's identity anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo? ContentMetaInfo { get; set; }

    /// <summary>
    /// The ID of the project this attribute set belongs to.
    /// </summary>
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// The human-readable name of the attribute set (e.g., "Core Stats").
    /// </summary>
    [MaxLength(128), Required]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// The sort order for displaying the attribute set in UI lists.
    /// </summary>
    public int DisplayOrder { get; set; } = 0;
    
    /// <summary>
    /// Indicates whether this attribute set is currently active and available for use.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property for the parent project.
    /// </summary>
    public virtual Project Project { get; set; }
}
