// =============================================================================
// ExportFormatEnum - Enum for content export formats supported by the engine
// =============================================================================

namespace GameDev.Core.Enums;

/// <summary>
/// Export formats available for exporting content items to game engines.
/// Supports JSON, CSV, XML GDD, and PDF output formats.
/// </summary>
public enum ExportFormatEnum
{
    /// <summary>
    /// JSON format for structured data export (Unity/Unreal)
    /// </summary>
    Json,
    
    /// <summary>
    /// CSV format for spreadsheet/data analysis export
    /// </summary>
    Csv,
    
    /// <summary>
    /// XML format for Game Design Document structure (GDD standard)
    /// </summary>
    XmlGdd,
    
    /// <summary>
    /// PDF format for documentation and presentation export
    /// </summary>
    Pdf
}
