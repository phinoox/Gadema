// =============================================================================
using GameDev.Core.Enums;
using GameDev.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Represents a lore entry (world-building information).
/// Used for documenting setting details, history, and mythology.
/// </summary>
public class LoreEntry
{
    /// <summary>
    /// Unique identifier for the lore entry.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? ContentItemId { get; set; }  
    
    [ForeignKey("ContentItemId")]
    public virtual ContentItem? ContentItem { get; set; }

    
    /// <summary>
    /// ID of the project this lore belongs to.
    /// </summary>
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }

    // Navigation property: Project (Many-to-One)
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }
    
    /// <summary>
    /// Type of lore (History, Mythology, Geography, etc.).
    /// </summary>
    [EnumDataType(typeof(LoreTypeEnum)), Required]
    public LoreTypeEnum LoreType { get; set; }
    
    /// <summary>
    /// Title of the lore entry.
    /// </summary>
    [MaxLength(128), Required]
    public string Title { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the lore entry (unique).
    /// </summary>
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    /// <summary>
    /// Content of the lore entry.
    /// </summary>
    [MaxLength(4096)]
    public string? Content { get; set; }
    
    /// <summary>
    /// Indicates if the lore entry is published.
    /// </summary>
    public bool Published { get; set; } = false;

}