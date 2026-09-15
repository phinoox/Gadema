// =============================================================================

using Gadema.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Templates;

/// <summary>
/// Represents a project template for quick project creation.
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class ProjectTemplate
{
    /// <summary>
    /// Unique identifier for the project template.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Name of the template (e.g., "Fantasy Book", "Action RPG").
    /// </summary>
    [Required]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the template (unique).
    /// </summary>
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    /// <summary>
    /// Description of the template.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Template type: 0=FantasyBook, 1=ActionRPG, 2=SciFi.
    /// </summary>
    public int TemplateType { get; set; }  // Enum: FantasyBook, ActionRPG, SciFi
    
    /// <summary>
    /// Indicates if the template is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}