// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Represents a tag for content organization and filtering.
/// Tags can be applied to both content items and media attachments.
/// </summary>
public class Tag
{
    /// <summary>
    /// Unique identifier for the tag.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Tag name (unique).
    /// </summary>
    [Required, Display(Name = "Tag Name")]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the tag (unique).
    /// </summary>
    [Column("slug"), MaxLength(128), Required]
    public string Slug { get; set; } = "";
    
    /// <summary>
    /// Tag description.
    /// </summary>
    [MaxLength(128)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Hex color code for tag visualization.
    /// </summary>
    [MaxLength(36)]
    public string? ColorHex { get; set; }
    
    /// <summary>
    /// Indicates if the tag is active.
    /// </summary>
    public bool IsActive { get; set; } 

    // Collection navigation properties (Many-to-Many through junction tables)
    /// <summary>
    /// Navigation property: Collection of content item associations via ContentTags junction table
    /// </summary>
    public virtual ICollection<ContentTags> ContentTagAssociations { get; set; } = new List<ContentTags>();

    /// <summary>
    /// Navigation property: Collection of media attachment associations via MediaTags junction table
    /// </summary>
    public virtual ICollection<MediaTags> MediaTagAssociations { get; set; } = new List<MediaTags>();

}