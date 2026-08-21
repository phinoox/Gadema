// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Represents an external reference (Google Docs, Pinterest boards, etc.).
/// Used for linking to external resources like GDD documents or art references.
/// </summary>
public class ExternalReference
{
    /// <summary>
    /// Unique identifier for the external reference.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Type of parent entity (0=ContentItem, 1=Task, 2=Comment).
    /// </summary>
    public int ParentType { get; set; }
    
    /// <summary>
    /// ID of the parent entity.
    /// </summary>
    public Guid? ParentId { get; set; }
    
    /// <summary>
    /// External URL (required).
    /// </summary>
    [Required, Display(Name = "External URL")]
    public string Url { get; set; } = "";
    
    /// <summary>
    /// Title of the external resource.
    /// </summary>
    [MaxLength(128)]
    public string Title { get; set; } = "";
    
    /// <summary>
    /// Type: 0=Document, 1=Image, 2=Video, 3=Audio.
    /// </summary>
    public int Type { get; set; }
    
    /// <summary>
    /// Indicates if the reference is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}