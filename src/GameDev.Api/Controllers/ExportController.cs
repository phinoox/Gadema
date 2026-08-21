// =============================================================================
using Microsoft.AspNetCore.Http;
// GameDev.Api - ASP.NET Core Web API Controllers
// =============================================================================

using System;
using System.Threading.Tasks;
using GameDev.Api.Services;
using GameDev.Core.Dtos.Export;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameDev.Api.Controllers;

/// <summary>
/// Controller for export endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{id}/export")]
public class ExportController : ControllerBase
{
    private readonly IExportService _exportService;
    private readonly ILogger<ExportController> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public ExportController(IExportService exportService, ILogger<ExportController> logger)
    {
        _exportService = exportService;
        _logger = logger;
    }

    /// <summary>
    /// Export to JSON format.
    /// </summary>
    [HttpPost("json")]
    public async Task<IActionResult> ExportToJsonAsync(Guid id, [FromBody] ExportJsonDto exportDto)
    {
        var response = await _exportService.ExportToJsonAsync(id, exportDto);
        return Ok(response);
    }

    /// <summary>
    /// Export to CSV format (Unity compatible).
    /// </summary>
    [HttpPost("csv")]
    public async Task<IActionResult> ExportToCsvAsync(Guid id, [FromBody] ExportCsvDto exportDto)
    {
        var response = await _exportService.ExportToCsvAsync(id, exportDto);
        return Ok(response);
    }

    /// <summary>
    /// Export to XML GDD format (Unreal compatible).
    /// </summary>
    [HttpPost("xml-gdd")]
    public async Task<IActionResult> ExportToXmlGddAsync(Guid id, [FromBody] ExportXmlGddDto exportDto)
    {
        var response = await _exportService.ExportToXmlGddAsync(id, exportDto);
        return Ok(response);
    }

    /// <summary>
    /// Export to PDF format (GDD document).
    /// </summary>
    [HttpPost("pdf")]
    public async Task<IActionResult> ExportToPdfAsync(Guid id, [FromBody] ExportPdfDto exportDto)
    {
        var response = await _exportService.ExportToPdfAsync(id, exportDto);
        return Ok(response);
    }
}