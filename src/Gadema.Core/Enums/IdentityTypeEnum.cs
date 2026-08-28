// =============================================================================
// Enums/IdentityTypeEnum.cs - Enum for identity types (race, faction, alignment, guild)
// =============================================================================

using System;
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Models;

namespace Gadema.Core.Enums;

/// <summary>
/// Types of character identities that can be configured per project.
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
    /// Character guild or organization.
    /// </summary>
    Guild = 8
}

[Flags]
public enum IdentityDefinitionType : int
{
    Race = 1,
    Faction = 2,
    Alignment = 4,
    Guild = 8
}