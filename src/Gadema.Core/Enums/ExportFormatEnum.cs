// =============================================================================
// ExportFormatEnum - Enum for content export formats supported by the engine
// =============================================================================

namespace Gadema.Core.Enums;

/// <summary>
/// Defines the supported output formats for exporting content from GaDeMa to external game engines or tools.
/// </summary>
public enum ExportFormatEnum
{
    /// <summary>Structured JSON format, ideal for importing into Unity or Unreal Engine databases.</summary>
    Json,
    
    /// <summary>Comma-Separated Values (CSV) format, suitable for spreadsheet-based data analysis and balancing.</summary>
    Csv,
    
    /// <summary>XML format following Game Design Document (GDD) standards for structured documentation.</summary>
    XmlGdd,
    
    /// <summary>PDF format for high-quality document presentation and offline reading.</summary>
    Pdf
}

