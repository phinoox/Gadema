// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Represents an ending definition for branching narratives.
/// </summary>
public class EndingDefinition
{
    /// <summary>
    /// Unique identifier for the ending definition.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the project this ending belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Title of the ending (e.g., "True Ending", "Bad Ending").
    /// </summary>
    [MaxLength(128), Required]
    public string EndingTitle { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the ending (unique).
    /// </summary>
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    /// <summary>
    /// Description of the ending.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Conditions JSON for triggering this ending.
    /// </summary>
    [MaxLength(1024)]
    public string? ConditionsJson { get; set; }  // JSON conditions for triggering ending
    
    /// <summary>
    /// Indicates if the ending is published.
    /// </summary>
    public bool Published { get; set; } = false;
}