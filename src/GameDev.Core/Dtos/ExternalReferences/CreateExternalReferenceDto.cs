// =============================================================================
using System.ComponentModel.DataAnnotations;

// =============================================================================

namespace GameDev.Core.Dtos.ExternalReferences;

/// <summary>
/// DTO for creating an external reference.
/// </summary>
public class CreateExternalReferenceDto
{
    /// <summary>
    /// ID of the parent content item (required).
    /// </summary>
    [Required]
    public Guid ContentItemId { get; set; }
    
    /// <summary>
    /// External URL (required).
    /// </summary>
    [Required, MaxLength(2048), Display(Name = "External URL")]
    public string Url { get; set; } = "";
    
    /// <summary>
    /// Title of the external resource (optional).
    /// </summary>
    [MaxLength(128)]
    public string? Title { get; set; } = null!;
    
    /// <summary>
    /// Type: 0=Document, 1=Image, 2=Video, 3=Audio.
    /// </summary>
    [Range(0, 3)]
    public int Type { get; set; } = 0;
}