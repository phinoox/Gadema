// =============================================================================
using GameDev.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Child entity for character details (FK as Primary Key pattern).
/// Stores detailed character information linked to ContentItem.
/// </summary>
public class CharacterDetails
{
    /// <summary>
    /// FK as Primary Key - links to ContentItem.Id
    /// </summary>
    public Guid ContentItemId { get; set; }  // FK as Primary Key

    // Navigation property: ContentItem (Many-to-One)
    [ForeignKey("ContentItemId")]
    public virtual ContentItem ContentItem { get; set; }

    /// <summary>
    /// Character name.
    /// </summary>
    [Required, Display(Name = "Character Name")]
    public string Name { get; set; } = "";

    /// <summary>
    /// ID of the class template for this character.
    /// </summary>
    public Guid? ClassTemplateId { get; set; }

    /// <summary>
    /// Character level.
    /// </summary>
    public int Level { get; set; } = 1;

    /// <summary>
    /// Role (Protagonist, Antagonist, etc.).
    /// </summary>
    public int? Role { get; set; }  // Enum: Protagonist, Antagonist, etc.

    /// <summary>
    /// Status (Alive, Deceased, Missing).
    /// </summary>
    public int? Status { get; set; }  // Enum: Alive, Deceased, Missing

    // Collection navigation property
    /// <summary>
    /// Navigation property: Collection of character background associations
    /// via CharacterDetailsCharacterBackground junction table.
    /// Foreign key: ContentItemId (matches FK in CharacterBackground)
    /// </summary>
    public virtual ICollection<CharacterBackground> CharacterBackgrounds { get; set; } = new List<CharacterBackground>();

}