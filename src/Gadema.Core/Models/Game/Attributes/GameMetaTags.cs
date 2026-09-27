using Gadema.Core.Utils;

namespace Gadema.Core.Models.Game.Attributes;

/// <summary>
/// Defines tags used for categorizing game-related effects and mechanics (e.g., elements or status types).
/// </summary>
[ModuleIndex(2)]
public enum GameTag : int
{
    /// <summary>Fire-based element or effect.</summary>
    Fire,    // Serial 0
    /// <summary>Water-based element or effect.</summary>
    Water,   // Serial 1
    /// <summary>A state of being unable to take actions.</summary>
    Stunned, // Serial 2
    /// <summary>A state of reduced movement or action speed.</summary>
    Slowed   // Serial 3
}

