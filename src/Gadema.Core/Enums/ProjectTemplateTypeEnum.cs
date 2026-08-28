using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Enums;

/// <summary>
/// Project template types for standardized project structures.
/// </summary>
public enum ProjectTemplateTypeEnum
{
    /// <summary>
    /// Fantasy book-style narrative (traditional prose format).
    /// </summary>
    [Display(Name = "Fantasy Book")]
    FantasyBook = 0,

    /// <summary>
    /// Action-oriented RPG project template.
    /// </summary>
    [Display(Name = "Action RPG")]
    ActionRPG = 1,

    /// <summary>
    /// Science fiction themed project template.
    /// </summary>
    [Display(Name = "Sci-Fi")]
    SciFi = 2,

    /// <summary>
    /// Horror/thriller narrative template.
    /// </summary>
    [Display(Name = "Horror/Thriller")]
    HorrorThriller = 3,

    /// <summary>
    /// Romance-focused project template.
    /// </summary>
    [Display(Name = "Romance")]
    Romance = 4,

    /// <summary>
    /// Mystery/investigation narrative template.
    /// </summary>
    [Display(Name = "Mystery")]
    Mystery = 5,

    /// <summary>
    /// Horror-focused project template.
    /// </summary>
    [Display(Name = "Horror")]
    Horror = 6,
}
