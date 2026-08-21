// =============================================================================
using GameDev.Core.Dtos;
using Microsoft.EntityFrameworkCore;
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GameDev.Core.Dtos.Export;
using GameDev.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GameDev.Api.Services;

/// <summary>
/// Implementation of export service.
/// </summary>
public class ExportService : IExportService
{
    private readonly GameDbContext _context;
    private readonly ILogger<ExportService> _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public ExportService(GameDbContext context, IConfiguration configuration, ILogger<ExportService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Export to JSON format.
    /// </summary>
    public async Task<IActionResult> ExportToJsonAsync(Guid projectId, ExportJsonDto exportDto)
    {
        var sections = JsonSerializer.Deserialize<string[]>(exportDto.Sections ?? "[]")?.ToList() ?? new List<string>();
        
        var projects = await _context.Projects
            .Where(p => p.Id == projectId)
            .Include(p => p.ContentItems)
                .ThenInclude(ci => ci.MediaAttachments)
                    .ThenInclude(ma => ma)
                .ThenInclude(ci => ci.ExternalReferences)
            .ToListAsync();

        var content = new
        {
            project = projects.First(),
            contentItems = sections.SelectMany(section => projects.First().ContentItems.Where(ci => ci.ContentType.ToString() == section)).ToList()
        };

        return Ok(JsonSerializer.Serialize(content), "application/json");
    }

    /// <summary>
    /// Export to CSV format.
    /// </summary>
    public async Task<IActionResult> ExportToCsvAsync(Guid projectId, ExportCsvDto exportDto)
    {
        var content = await _context.ContentItems
            .Where(ci => ci.ProjectId == projectId && ci.ContentType == exportDto.ContentType && ci.Published)
            .Select(ci => new
            {
                Title = ci.Title,
                Slug = ci.Slug,
                Description = ci.Description,
                Published = ci.Published,
                Status = ci.Status
            })
            .ToListAsync();

        var csvContent = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("Title,Slug,Description,Published,Status\n"));
        return File(csvContent, "text/csv", $"export-{projectId}.csv");
    }

    /// <summary>
    /// Export to XML GDD format.
    /// </summary>
    public async Task<IActionResult> ExportToXmlGddAsync(Guid projectId, ExportXmlGddDto exportDto)
    {
        var content = await _context.ContentItems
            .Where(ci => ci.ProjectId == projectId && ci.ContentType == exportDto.ContentType && ci.Published)
            .Select(ci => new
            {
                Id = ci.Id,
                Title = ci.Title,
                Description = ci.Description,
                Slug = ci.Slug
            })
            .ToListAsync();

        var xmlContent = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("<GDDExport><ContentItems/></GDDExport>"));
        return File(xmlContent, "application/xml", $"export-{projectId}.xml");
    }

    /// <summary>
    /// Export to PDF format.
    /// </summary>
    public async Task<IActionResult> ExportToPdfAsync(Guid projectId, ExportPdfDto exportDto)
    {
        var content = await _context.ContentItems
            .Where(ci => ci.ProjectId == projectId)
            .ToListAsync();

        return Ok($"PDF Content for project {projectId}");
    }
}