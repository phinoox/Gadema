// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Base.Enums;

/// <summary>
/// Defines the various types of content that can be tracked within the GaDeMa system.
/// </summary>
public enum ContentTypeEnum
{
    /// <summary>Unspecified or other type of content.</summary>
    Other = 0,
    /// <summary>A character entity.</summary>
    Character = 1,
    
    /// <summary>A world or geographic location.</summary>
    World = 2,
    
    /// <summary>A game mechanic or system component.</summary>
    Mechanic = 3,
    
    /// <summary>A key plot point in a narrative.</summary>
    PlotPoint = 4,
    
    /// <summary>A quest or mission.</summary>
    Quest = 5,
    
    /// <summary>An item or artifact within the setting.</summary>
    Item = 6,
    
    /// <summary>A faction or organized group.</summary>
    Faction = 7,
    
    /// <summary>An enemy or monster entity.</summary>
    Enemy = 8,
    
    /// <summary>An ability or skill set.</summary>
    Ability = 9,
    
    /// <summary>A spell or magical element.</summary>
    Spell = 10,
    
    /// <summary>A vehicle or mode of transport.</summary>
    Vehicle = 11,
    
    /// <summary>A narrative scene.</summary>
    Scene = 12,

    /// <summary>An entry in a lore database.</summary>
    LoreEntry = 13,

    /// <summary>A high-level story outline.</summary>
    StoryOutline = 14,

    /// <summary>A major narrative beat or milestone.</summary>
    StoryBeat = 15,

    /// <summary>A specific location within a world.</summary>
    WorldLocation = 16,

    /// <summary>The current state of a character.</summary>
    CharacterState = 17,

    /// <summary>An identity definition for authentication or tracking.</summary>
    IdentityDefinition = 18,

    /// <summary>A reference to external data or systems.</summary>
    ExternalReference = 19,

    /// <summary>A content review record.</summary>
    ContentReview = 20,

    /// <summary>A series of related projects.</summary>
    ProjectSeries = 21,

    /// <summary>A user-provided comment on a piece of content.</summary>
    Comment = 22,

    /// <summary>A segment within a scene.</summary>
    SceneSegment = 23
}