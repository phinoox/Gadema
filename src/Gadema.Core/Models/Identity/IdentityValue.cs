namespace Gadema.Core.Models.Identity;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a specific option within an identity category (e.g., "Human" within the "Race" definition).
/// This entity serves as a selectable value for assigning traits to characters or other content items.
/// </summary>
[ModelDependency(typeof(IdentityDefinition), typeof(ContentMetaInfo))]
public class IdentityValue
{
    /// <summary>
    /// Unique identifier for the identity value.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    // --- Identity Anchor (The "Soul") ---
    /// <summary>
    /// The ID of the associated ContentMetaInfo entity acting as the anchor.
    /// </summary>
    [Required]
    public Guid MetaInfoId { get; set; }
    /// <summary>
    /// Navigation property for the content meta information anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    // --- Domain Properties (The "Body") ---
    /// <summary>
    /// The ID of the identity definition this value belongs to.
    /// </summary>
    [Required]
    public Guid IdentityDefinitionId { get; set; }
    /// <summary>
    /// Navigation property for the parent identity definition.
    /// </summary>
    [ForeignKey("IdentityDefinitionId")]
    public virtual IdentityDefinition IdentityDefinition { get; set; } = null!;

    /// <summary>
    /// The human-readable name of the value (e.g., "Human").
    /// </summary>
    [MaxLength(128), Required]
    public string Name { get; set; } = "";

    /// <summary>
    /// A URL-friendly slug for the value, used in routing and lookups.
    /// </summary>
    [MaxLength(128), Column("slug")]
    public string Slug { get; set; } = "";

    /// <summary>
    /// An optional detailed description of this specific identity value.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }

    /// <summary>
    /// The sort order for this value within its parent definition's list.
    /// </summary>
    public int OrderIndex { get; set; } = 0;

    /// <summary>
    /// Indicates if this is the default selection when no other value is specified.
    /// </summary>
    public bool IsDefault { get; set; } = false;
}
