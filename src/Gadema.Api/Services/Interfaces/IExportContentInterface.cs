// =============================================================================
using System;
using System.Threading.Tasks;
using Gadema.Core.Dtos.Export;
// Gadema.Api - ASP.NET Core Web API Services
// =============================================================================

namespace Gadema.Api.Services;

/// <summary>
/// Interface for export content service.
/// Handles actual file content generation (JSON, CSV, XML strings and PDF bytes).
/// Reusable for GDD downloads, bulk operations, etc.
/// </summary>
public interface IExportContentService
{
    /// <summary>
    /// Generate JSON export content as string.
    /// </summary>
    Task<string> GenerateJsonExportAsync(Guid projectId, ExportJsonDto dto);

    /// <summary>
    /// Generate CSV export content as string.
    /// </summary>
    Task<string> GenerateCsvExportAsync(Guid projectId, ExportCsvDto dto);

    /// <summary>
    /// Generate XML GDD export content as string.
    /// </summary>
    Task<string> GenerateXmlExportAsync(Guid projectId, ExportXmlGddDto dto);

    /// <summary>
    /// Generate PDF export content as bytes (for QuestPDF integration).
    /// </summary>
    Task<byte[]> GeneratePdfExportAsync(Guid projectId, ExportPdfDto dto);
}