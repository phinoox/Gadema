// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Child entity for character background (FK as Primary Key pattern).
/// Stores detailed character biography and personality traits.
/// </summary>
public class CharacterBackground
{
    /// <summary>
    /// FK as Primary Key - links to ContentItem.Id
    /// </summary>
    public Guid ContentItemId { get; set; }  // FK as Primary Key
    
    /// <summary>
    /// Full biography of the character.
    /// </summary>
    [MaxLength(4096)]
    public string? FullBiography { get; set; }
    
    /// <summary>
    /// Personality traits (JSON array).
    /// </summary>
    [MaxLength(4096)]
    public string? PersonalityTraits { get; set; }  // JSON array
    
    /// <summary>
    /// Character motivation.
    /// </summary>
    [MaxLength(2048)]
    public string? Motivation { get; set; }
    
    /// <summary>
    /// Character conflict.
    /// </summary>
    [MaxLength(2048)]
    public string? Conflict { get; set; }
    
    /// <summary>
    /// Voice notes for character portrayal.
    /// </summary>
    [MaxLength(4096)]
    public string? VoiceNotes { get; set; }
    
    /// <summary>
    /// Key events (JSON array).
    /// </summary>
    [MaxLength(4096)]
    public string? KeyEvents { get; set; }  // JSON array
    
    /// <summary>
    /// Indicates if the background is published.
    /// </summary>
    public bool Published { get; set; } = false;
    
    /// <summary>
    /// Version number for tracking changes.
    /// </summary>
    public int Version { get; set; } = 0;
}