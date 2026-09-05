// =============================================================================
using Gadema.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Represents a background element for a character.
/// </summary>
[DependencyResolver.ModelDependency(typeof(ContentItem))]
public class CharacterBackground
{
    /// <summary>
    /// Unique identifier for the character background.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the content item this background belongs to.
    /// </summary>
    [Required]
    public Guid ContentItemId { get; set; }
    
    // Navigation property: ContentItem (Many-to-One)
    [ForeignKey("ContentItemId")]
    public virtual ContentItem ContentItem { get; set; }

    /// <summary>
    /// ID of the character details this background is associated with.
    /// </summary>
    [Required]
    public Guid CharacterDetailsId { get; set; }

    // Navigation property: CharacterDetails (Many-to-One)
    [ForeignKey("CharacterDetailsId")]
    public virtual CharacterDetails CharacterDetails { get; set; }
    /// <summary>
    /// Description of the background.
    /// </summary>
    [MaxLength(2048)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Indicates if the background is published.
    /// </summary>
    public bool Published { get; set; } = false;
    
}