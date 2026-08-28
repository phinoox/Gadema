// =============================================================================
using Gadema.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Represents an attribute set for character/class templates.
/// Used for defining ability systems and character stats.
/// </summary>
public class AttributeSet
{
    /// <summary>
    /// Unique identifier for the attribute set.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? ContentItemId { get; set; }  
    
    [ForeignKey("ContentItemId")]
    public virtual ContentItem? ContentItem { get; set; }

    
    /// <summary>
    /// ID of the project this attribute set belongs to.
    /// </summary>
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Name of the attribute set.
    /// </summary>
    [MaxLength(128), Required]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// Display order for sorting.
    /// </summary>
    public int DisplayOrder { get; set; } = 0;
    
    /// <summary>
    /// Indicates if the attribute set is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property: Project (Cascade delete)
    /// Foreign key: ProjectId (matches FK in configuration)
    /// </summary>
    public virtual Project Project { get; set; }
}