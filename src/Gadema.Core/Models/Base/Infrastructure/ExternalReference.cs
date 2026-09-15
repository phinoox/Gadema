// =============================================================================
using Gadema.Core.Models.Base.MetaInfo;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Base.Infrastructure;

/// <summary>
/// Represents an external reference (Google Docs, Pinterest boards, etc.).
/// Used for linking to external resources like GDD documents or art references.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))]
public class ExternalReference
{
    /// <summary>
    /// Unique identifier for the external reference.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // --- Identity Anchor (The "Soul") ---
    [Required]
    public Guid MetaInfoId { get; set; }

    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    // --- Domain Properties (The "Body") ---

    /// <summary>
    /// The type of reference (e.g., Document, Image, Video).
    /// </summary>
    [Required]
    public ExternalReferenceTypeEnum ReferenceType { get; set; } = ExternalReferenceTypeEnum.Document;

    /// <summary>
    /// External URL.
    /// </summary>
    [Required, MaxLength(2048)]
    public string Url { get; set; } = "";
    
    /// <summary>
    /// Title of the external resource.
    /// </summary>
    [MaxLength(128)]
    public string? Title { get; set; }
    
    /// <summary>
    /// Author or creator of the reference material.
    /// </summary>
    [MaxLength(128)]
    public string? Author { get; set; }

    /// <summary>
    /// Additional notes or context about this reference.
    /// </summary>
    [MaxLength(4096)]
    public string? Notes { get; set; }

    /// <summary>
    /// Indicates if the reference is currently active/relevant.
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Self-referencing hierarchy (if needed for nested references)
    [MaxLength(128)]
    public string? Slug { get; set; } 
}