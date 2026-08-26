// =============================================================================
using GameDev.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Represents an ability set for character/class templates.
/// Used for defining ability systems and character skills (GAS-like architecture).
/// </summary>
public class AbilitySet
{
    /// <summary>
    /// Unique identifier for the ability set.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? ContentItemId { get; set; }  
    
    [ForeignKey("ContentItemId")]
    public virtual ContentItem? ContentItem { get; set; }

    
    /// <summary>
    /// Name of the ability set.
    /// </summary>
    [Required]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the ability set (unique).
    /// </summary>
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    /// <summary>
    /// Description of the ability set.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    /// <summary>
    /// ID of the project this ability set belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    // Navigation property for Project (Many-to-One)
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }
    
    /// <summary>
    /// Type: 0=Combat, 1=Non-Combat, 2=Hybrid.
    /// </summary>
    public int Type { get; set; }  // Enum: Combat, Non-Combat, Hybrid
    
    /// <summary>
    /// Indicates if the ability set is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}