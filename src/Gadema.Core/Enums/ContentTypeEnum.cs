// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Enums;

/// <summary>
/// Content types for content items (characters, worlds, mechanics, etc.).
/// </summary>
public enum ContentTypeEnum
{
    /// <summary>Character</summary>
    Character = 0,
    
    /// <summary>World/Location</summary>
    World = 1,
    
    /// <summary>Mechanic/System</summary>
    Mechanic = 2,
    
    /// <summary>Plot Point</summary>
    PlotPoint = 3,
    
    /// <summary>Quest</summary>
    Quest = 4,
    
    /// <summary>Item/Artifact</summary>
    Item = 5,
    
    /// <summary>Faction</summary>
    Faction = 6,
    
    /// <summary>Enemy/Monster</summary>
    Enemy = 7,
    
    /// <summary>Ability/Skill</summary>
    Ability = 8,
    
    /// <summary>Spell/Magic</summary>
    Spell = 9,
    
    /// <summary>Vehicle/Transport</summary>
    Vehicle = 10,
    
    /// <summary>Other</summary>
    Other = 99,
    Scene = 12
}