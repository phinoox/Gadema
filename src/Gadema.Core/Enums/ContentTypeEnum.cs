// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Enums;

/// <summary>
/// Content types for content items (characters, worlds, mechanics, etc.).
/// </summary>
public enum ContentTypeEnum
{
     /// <summary>Other</summary>
    Other = 0,
    /// <summary>Character</summary>
    Character = 1,
    
    /// <summary>World/Location</summary>
    World = 2,
    
    /// <summary>Mechanic/System</summary>
    Mechanic = 3,
    
    /// <summary>Plot Point</summary>
    PlotPoint = 4,
    
    /// <summary>Quest</summary>
    Quest = 5,
    
    /// <summary>Item/Artifact</summary>
    Item = 6,
    
    /// <summary>Faction</summary>
    Faction = 7,
    
    /// <summary>Enemy/Monster</summary>
    Enemy = 8,
    
    /// <summary>Ability/Skill</summary>
    Ability = 9,
    
    /// <summary>Spell/Magic</summary>
    Spell = 10,
    
    /// <summary>Vehicle/Transport</summary>
    Vehicle = 11,
    
   
    Scene = 12,
    LoreEntry = 13,
    StoryOutline = 14,
    StoryBeat = 15,
    WorldLocation = 16,
    CharacterState = 17,
    IdentityDefinition = 18,
    ExternalReference = 19,
    ContentReview = 20,
    ProjectSeries = 21,
    Comment = 22,
    SceneSegment = 23
}