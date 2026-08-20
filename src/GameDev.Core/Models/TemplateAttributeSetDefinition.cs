// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Template attribute set definition for project templates.
/// </summary>
public class TemplateAttributeSetDefinition
{
    /// <summary>
    /// Unique identifier for the template attribute set definition.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the project template this definition belongs to.
    /// </summary>
    [Required]
    public Guid ProjectTemplateId { get; set; }
    
    /// <summary>
    /// ID of the attribute set definition.
    /// </summary>
    [Required]
    public Guid AttributeSetDefinitionId { get; set; }
}