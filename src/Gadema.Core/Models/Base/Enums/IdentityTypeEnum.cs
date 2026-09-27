// =============================================================================
// Enums/IdentityTypeEnum.cs - Enum for identity types (race, faction, alignment, guild)
// =============================================================================

using Gadema.Core.Models.Identity;

namespace Gadema.Core.Models.Base.Enums;

/// <summary>
/// Defines the various types of character identities that can be configured within a project, using bitwise flags.
/// </summary>
[Flags]
public enum IdentityTypeEnum : int
{
    /// <summary>
    /// Character race or species (e.g., Human, Elf, Orc).
    /// </summary>
    [EnumDataType(typeof(IdentityDefinition))]
    Race = 1,
    
    /// <summary>
    /// Character faction or allegiance group.
    /// </summary>
    Faction = 2,
    
    /// <summary>
    /// Character alignment (e.g., Lawful Good, Chaotic Evil).
    /// </summary>
    Alignment = 4,
    
    /// <summary>
    /// Character guild or organization membership.
    /// </summary>
    Guild = 8
}

/// <summary>
/// Specifies the type of identity definition being created.
/// </summary>
[Flags]
public enum IdentityDefinitionType : int
{
    /// <summary>Race-related identity definition.</summary>
    Race = 1,
    /// <summary>Faction-related identity definition.</summary>
    Faction = 2,
    /// <summary>Alignment-related identity definition.</summary>
    Alignment = 4,
    /// <summary>Guild-related identity definition.</summary>
    Guild = 8
}