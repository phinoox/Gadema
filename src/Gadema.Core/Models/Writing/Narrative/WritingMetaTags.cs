using Gadema.Core.Utils;

namespace Gadema.Core.Models.Writing.Narrative;

/// <summary>
/// Defines tags used for categorizing and filtering writing content (e.g., genres or themes).
/// </summary>
[ModuleIndex(1)]
public enum WritingTag : int
{
    /// <summary>High fantasy or magical settings.</summary>
    Fantasy, // Serial 0
    /// <summary>Science fiction or futuristic settings.</summary>
    SciFi,   // Serial 1
    /// <summary>Horror and suspenseful elements.</summary>
    Horror,  // Serial 2
    /// <summary>Dark fantasy or grimdark themes.</summary>
    Dark     // Serial 3
}

