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
using Gadema.Data.Database;
using Microsoft.Extensions.Logging;
using Gadema.Core.Services;

namespace Gadema.Api.Services;
public class ExportContentService : IGademaService,  IExportContentService
{
    private readonly GameDbContext _context;

    public ServiceTypeEnum ServiceType => ServiceTypeEnum.ExportContentService;

    public async Task<string> GenerateJsonExportAsync(Guid projectId, ExportJsonDto dto)
    {
        var projects = await _context.Projects
            .Where(p => p.Id == projectId)
            .Include(p => p.ContentItems)
                //.ThenInclude(ci => ci.MediaAttachments)
                //    .ThenInclude(ma => ma.ExternalReferences)  // ✅ Correct navigation
            .ToListAsync();

        var content = new
        {
            project = projects.First(),
            contentItems = dto.Sections?.Split(',') ?? []
        };

        return JsonSerializer.Serialize(content);
    }

    public async Task<string> GenerateCsvExportAsync(Guid projectId, ExportCsvDto dto)
    {
      var items = _context.ContentItems
            .Where(ci => ci.ProjectId == projectId && 
                        ci.ContentType == dto.ContentType && 
                        ci.Published)
            .Select(ci => new[] {
                $"{ci.Title}",
                $"{ci.Slug}",
                $"{(ci.Description ?? "")}",
                $"{ci.Published}"
            })
            .ToList();

        var csvContent = string.Join(",", items.Select(row => String.Join(",", row)));
        return csvContent;
    }

    public async Task<string> GenerateXmlExportAsync(Guid projectId, ExportXmlGddDto dto)
    {
         var items = _context.ContentItems
            .Where(ci => ci.ProjectId == projectId && 
                        ci.ContentType == dto.ContentType && 
                        ci.Published)
            .Select(ci => new
            {
                Id = ci.Id,
                Title = ci.Title,
                Description = ci.Description ?? ""
            })
            .ToList();

        var xmlContent = $"<GDDExport><ProjectId>{projectId}</ProjectId>" +
            string.Join("", items.Select(x => 
                $"<Item><Id>{x.Id}</Id><Title>{x.Title}</Title></Item>")) +
            "</GDDExport>";
        
        return xmlContent;
    }

    public async Task<byte[]> GeneratePdfExportAsync(Guid projectId, ExportPdfDto dto)
    {
        // TODO: Use QuestPDF library to generate PDF bytes
        return Array.Empty<byte>();  // Placeholder
    }
}

