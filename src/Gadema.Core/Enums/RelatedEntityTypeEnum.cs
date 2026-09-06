using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Enums;

/// <summary>
/// Related entity types for activity logging and cross-entity tracking.
/// </summary>
public enum RelatedEntityTypeEnum
{
    /// <summary>MetaInfo (characters, worlds, mechanics, etc.)</summary>
    [Display(Name = "Content Item")]
    MetaInfo = 0,

    /// <summary>ProjectTask (tasks and workflows)</summary>
    [Display(Name = "Task")]
    ProjectTask = 1,

    /// <summary>Project (projects themselves)</summary>
    [Display(Name = "Project")]
    Project = 2,

    /// <summary>StorySequence (narrative sequences)</summary>
    [Display(Name = "Story Sequence")]
    StorySequence = 3,

    /// <summary>AbilitySet (combat/ability systems)</summary>
    [Display(Name = "Ability Set")]
    AbilitySet = 4,

    /// <summary>Character (character-specific data)</summary>
    [Display(Name = "Character")]
    Character = 5,

    /// <summary>InventoryItem (game inventory)</summary>
    [Display(Name = "Inventory Item")]
    InventoryItem = 6,

    /// <summary>Template (project templates)</summary>
    [Display(Name = "Template")]
    Template = 7,
}
