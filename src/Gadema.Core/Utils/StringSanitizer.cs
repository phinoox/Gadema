using System.Text.RegularExpressions;

namespace Gadema.Core.Utils;

public static class StringSanitizer
{
    /// <summary>
    /// Converts a human-readable string into a web-friendly slug (e.s. "Dark Tone" -> "dark_tone").
    /// </summary>
    public static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        // 1. Convert to lowercase
        string result = input.ToLowerInvariant();

        // 2. Replace spaces and special characters with underscores
        result = Regex.Replace(result, @"[^a-z0-9]+", "_");

        // 3. Trim leading/trailing underscores
        return result.Trim('_');
    }
}