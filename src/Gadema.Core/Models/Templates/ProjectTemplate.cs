// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Templates;

/// <summary>
/// Represents a blueprint for creating new projects, defining preset structures and content requirements.
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class ProjectTemplate
{
    /// <summary>
    /// Unique identifier for the project template.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// The human-readable name of the template (e.g., "Fantasy Novel", "RPG Campaign").
    /// </summary>
    [Required]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// A unique, URL-friendly slug for the template.
    /// </summary>
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    /// <summary>
    /// A detailed description of what this template is designed for and its primary use cases.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    /// <summary>
    /// The category of the template, represented as an integer mapping to a template type enum (e.g., FantasyBook, ActionRPG).
    /// </summary>
    public int TemplateType { get; set; }  // Enum: FantasyBook, ActionRPG, SciFi
    
    /// <summary>
    /// Indicates whether this template is currently available for selection during project creation.
    /// </summary>
    public bool IsActive { get; set; } = true;
}