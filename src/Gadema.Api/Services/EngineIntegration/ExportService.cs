// =============================================================================
using Gadema.Core.Dtos.Export;
using Microsoft.EntityFrameworkCore;
// Gadema.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Gadema.Data;
using Microsoft.Extensions.Logging;
using Gadema.Core.Services;

namespace Gadema.Api.Services;

/// <summary>
/// Implementation of export service.
/// </summary>
public class ExportService : IGademaService,  IExportService
{
    private readonly ILogger<ExportService> _logger;

    public ServiceTypeEnum ServiceType => ServiceTypeEnum.ExportService;

    public ExportService(ILogger<ExportService> logger) => _logger = logger;
    public async Task<ExportJsonResponseDto> ExportToJsonAsync(Guid projectId, ExportJsonDto dto)
    {
        var response = new ExportJsonResponseDto
        {
            ProjectId = projectId,
            FileName = $"export-{projectId}-{DateTime.UtcNow:yyyyMMddHHmmss}.json",
            ContentType = "application/json"
        };
        return response;
    }

    public async Task<ExportCsvResponseDto> ExportToCsvAsync(Guid projectId, ExportCsvDto dto)
    {
        var response = new ExportCsvResponseDto
        {
            ProjectId = projectId,
            FileName = $"export-{projectId}.csv",
            ContentType = "text/csv"
        };
        return response;
    }

    public async Task<ExportXmlGddResponseDto> ExportToXmlGddAsync(Guid projectId, ExportXmlGddDto exportDto)
    {
        var response = new ExportXmlGddResponseDto
        {
            ProjectId = projectId,
            FileName = $"export-{projectId}-{DateTime.UtcNow:yyyyMMddHHmmss}.json",
            ContentType = "application/xml"
        };
        return response;
    }

    public async Task<ExportPdfResponseDto> ExportToPdfAsync(Guid projectId, ExportPdfDto exportDto)
    {
        var response = new ExportPdfResponseDto
        {
            ProjectId = projectId,
            FileName = $"export-{projectId}-{DateTime.UtcNow:yyyyMMddHHmmss}.json",
            ContentType = "application/pdf"
        };
        return response;
    }

    // Similar for XML and PDF...
}