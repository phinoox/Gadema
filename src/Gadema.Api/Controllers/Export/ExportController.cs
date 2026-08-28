// =============================================================================
using Microsoft.AspNetCore.Http;
// Gadema.Api - ASP.NET Core Web API Controllers
// =============================================================================

using System;
using System.Threading.Tasks;
using Gadema.Api.Services;
using Gadema.Core.Dtos.Export;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gadema.Api.Controllers;

/// <summary>
/// Controller for export endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{id}/export")]
public class ExportController : ControllerBase
{
    private readonly IExportService _exportService;
    private readonly IExportContentService _contentService;

    public ExportController(IExportService exportService, IExportContentService contentService)
    {
        _exportService = exportService;
        _contentService = contentService;
    }

    [HttpPost("json")]
    public async Task<IActionResult> ExportToJsonAsync(Guid id, [FromBody] ExportJsonDto dto)
    {
        var response = await _exportService.ExportToJsonAsync(id, dto);
        
        // Generate file content using ContentService (reusable elsewhere)
        var jsonContent = await _contentService.GenerateJsonExportAsync(id, dto);
        
        return File(
            System.Text.Encoding.UTF8.GetBytes(jsonContent),
            response.ContentType,
            response.FileName
        );
    }

    [HttpPost("csv")]
    public async Task<IActionResult> ExportToCsvAsync(Guid id, [FromBody] ExportCsvDto dto)
    {
        var response = await _exportService.ExportToCsvAsync(id, dto);
        var csvContent = await _contentService.GenerateCsvExportAsync(id, dto);

        return File(
            System.Text.Encoding.UTF8.GetBytes(csvContent),
            response.ContentType,
            response.FileName
        );
    }

    [HttpPost("xml-gdd")]
    public async Task<IActionResult> ExportToXmlGddAsync(Guid id, [FromBody] ExportXmlGddDto dto)
    {
        var response = await _exportService.ExportToXmlGddAsync(id, dto);
        var xmlContent = await _contentService.GenerateXmlExportAsync(id, dto);

        return File(
            System.Text.Encoding.UTF8.GetBytes(xmlContent),
            response.ContentType,
            response.FileName
        );
    }

    [HttpPost("pdf")]
    public async Task<IActionResult> ExportToPdfAsync(Guid id, [FromBody] ExportPdfDto dto)
    {
        var response = await _exportService.ExportToPdfAsync(id, dto);
        // In production: return QuestPDF-generated PDF bytes
        // For now: placeholder
        return Ok("PDF generation endpoint");
    }
}