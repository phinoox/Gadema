// =============================================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gadema.Core.Models.Base.MetaInfo;
// =============================================================================

namespace Gadema.Core.Models.Identity;

/// <summary>
/// A specific instance of an identity type (e.g., "Human" for the "Race" definition).
/// This is a component that belongs to an IdentityDefinition anchor.
/// </summary>
[ModelDependency(typeof(IdentityDefinition), typeof(ContentMetaInfo))]
public class IdentityValue
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // --- Identity Anchor (The "Soul") ---
    [Required]
    public Guid MetaInfoId { get; set; }

    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    // --- Domain Properties (The "Body") ---

    [Required]
    public Guid IdentityDefinitionId { get; set; }

    [ForeignKey("IdentityDefinitionId")]
    public virtual IdentityDefinition IdentityDefinition { get; set; }

    /// <summary>
    /// The human-readable name of the value (e.g., "Human").
    /// </summary>
    [MaxLength(128), Required]
    public string Name { get; set; } = "";

    /// <summary>
    /// URL-friendly slug for the value.
    /// </summary>
    [MaxLength(128), Column("slug")]
    public string Slug { get; set; } = "";

    /// <summary>
    /// Optional description of this specific value.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }

    /// <summary>
    /// The order in which this value appears in lists.
    /// </summary>
    public int OrderIndex { get; set; } = 0;

    /// <summary>
    /// Indicates if this is the default selection for this identity type.
    /// </summary>
    public bool IsDefault { get; set; } = false;
}