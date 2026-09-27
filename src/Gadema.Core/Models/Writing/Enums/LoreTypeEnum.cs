// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Writing.Enums;

/// <summary>
/// Defines the categories of lore entries used for world-building.
/// </summary>
public enum LoreTypeEnum
{
    /// <summary>Historical events, timelines, and chronicles.</summary>
    History = 0,
    
    /// <summary>Legends, myths, and folklore that shape a culture's identity.</summary>
    Mythology = 1,
    
    /// <summary>Physical landscapes, terrain, climates, and locations.</summary>
    Geography = 2,
    
    /// <summary>Customs, traditions, languages, and social behaviors.</summary>
    Culture = 3,
    
    /// <summary>Scientific advancements, tools, and technical capabilities of a society.</summary>
    Technology = 4,
    
    /// <summary>Social structures, hierarchies, and community organization.</summary>
    Society = 5,
    
    /// <summary>Trade systems, resources, and financial structures.</summary>
    Economy = 6,
    
    /// <summary>Governance, laws, and political power dynamics.</summary>
    Politics = 7,
    
    /// <summary>Belief systems, deities, and spiritual practices.</summary>
    Religion = 8,
    
    /// <summary>Miscellaneous lore information that does not fit other categories.</summary>
    Other = 9
}