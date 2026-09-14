// =============================================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Base.MetaInfo;

/// <summary>
/// The universal identity anchor for all entities in the GaDeMa universe.
/// Provides the core "DNA" (Identity) that remains consistent across any scope.
/// </summary>
public abstract class BaseMetaInfo
{
    /// <summary>
    /// Unique identifier for the entity.
    /// </summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The primary display name of the entity.
    /// </summary>
    [Required, MaxLength(128)]
    public string Title { get; set; } = "";

    /// <summary>
    /// URL-friendly slug for the entity (unique).
    /// </summary>
    [Required, MaxLength(128), Column("slug")]
    public string Slug { get; set; } = "";

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last modification timestamp.
    /// </summary>
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(4096)]
    public string? ShortDesc { get; set; }

    public bool IsPublic { get; set; } = false;
}
