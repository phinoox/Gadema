// =============================================================================
using System.ComponentModel.DataAnnotations;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Base.Projects.Enums;

/// <summary>
/// Enum for primary project format. Determines default UI emphasis and workspace layout.
/// </summary>
public enum PrimaryFormatEnum
{
    /// <summary>Book or novel format</summary>
    [Display(Name = "Book")]
    Book = 0,
    
    /// <summary>Manga or comic format</summary>
    [Display(Name = "Manga")]
    Manga = 1,
    
    /// <summary>Interactive game format</summary>
    [Display(Name = "Game")]
    Game = 2,
    
    /// <summary>Hybrid format (e.g., visual novel, interactive fiction)</summary>
    [Display(Name = "Hybrid")]
    Hybrid = 3
}

/// <summary>
/// Enum for project tone. Influences pacing and emotional beat suggestions.
/// </summary>
public enum ToneEnum
{
    /// <summary>Neutral or undefined tone</summary>
    [Display(Name = "Neutral")]
    Neutral = 0,
    
    /// <summary>Dark, serious, or grim tone</summary>
    [Display(Name = "Dark")]
    Dark = 1,
    
    /// <summary>Light, uplifting, or positive tone</summary>
    [Display(Name = "Light")]
    Light = 2,
    
    /// <summary>Humorous or comedic tone</summary>
    [Display(Name = "Humorous")]
    Humorous = 3,
    
    /// <summary>Tense or suspenseful tone</summary>
    [Display(Name = "Tense")]
    Tense = 4,
    
    /// <summary>Romantic or emotional tone</summary>
    [Display(Name = "Romantic")]
    Romantic = 5
}

/// <summary>
/// Enum for target audience. Influences content warnings and complexity suggestions.
/// </summary>
public enum AudienceEnum
{
    /// <summary>All ages</summary>
    [Display(Name = "All Ages")]
    AllAges = 0,
    
    /// <summary>Children (under 12)</summary>
    [Display(Name = "Children")]
    Children = 1,
    
    /// <summary>Teenagers (12-17)</summary>
    [Display(Name = "Teen")]
    Teen = 2,
    
    /// <summary>Adults (18+)</summary>
    [Display(Name = "Adult")]
    Adult = 3
}
