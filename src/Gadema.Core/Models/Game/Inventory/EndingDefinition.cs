// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

using Gadema.Core.Models.Base.Projects;

namespace Gadema.Core.Models.Game.Inventory;

/// <summary>
/// Represents a predefined narrative ending within a project.
/// Defines the criteria (via conditions) and description for how a player/character reaches this outcome.
/// </summary>
[ModelDependency(typeof(Project))]
public class EndingDefinition
{
    /// <summary>
    /// Unique identifier for the ending definition.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// The ID of the project this ending belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Navigation property for the parent project.
    /// </summary>
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }

    /// <summary>
    /// The human-readable title of the ending (e.g., "The Hero's Triumph", "Dark Descent").
    /// </summary>
    [MaxLength(128), Required]
    public string Title { get; set; } = "";
    
    /// <summary>
    /// A unique, URL-friendly slug for the ending.
    /// </summary>
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    /// <summary>
    /// A detailed description of what happens in this ending.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    /// <summary>
    /// JSON representation of the requirements or logic (e.g., flag checks, stats) needed to trigger this ending.
    /// </summary>
    [MaxLength(1024)]
    public string? ConditionsJson { get; set; }  // JSON conditions for triggering ending
    
    /// <summary>
    /// Indicates if the ending is officially published and available in the game world.
    /// </summary>
    public bool Published { get; set; } = false;
}