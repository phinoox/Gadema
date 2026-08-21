// =============================================================================
using GameDev.Core.Dtos;
using Microsoft.EntityFrameworkCore;
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GameDev.Core.Dtos.ExternalReferences;
using GameDev.Core.Models;
using GameDev.Data;
using Microsoft.Extensions.Logging;

namespace GameDev.Api.Services;

/// <summary>
/// Implementation of external reference service.
/// </summary>
public class ExternalReferenceService : IExternalReferenceService
{
    private readonly GameDbContext _context;
    private readonly ILogger<ExternalReferenceService> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public ExternalReferenceService(GameDbContext context, ILogger<ExternalReferenceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// List external references for content item.
    /// </summary>
    public async Task<ApiResponseDto<ReferenceListResponseDto>> GetReferencesAsync(Guid contentItemId)
    {
        var references = await _context.ExternalReferences
            .Where(r => r.ParentType == 0 && r.ParentId == contentItemId && r.IsActive)
            .ToListAsync();

        return ApiResponseDto<ReferenceListResponseDto>.Success(new ReferenceListResponseDto());
    }

    /// <summary>
    /// Create external reference.
    /// </summary>
    public async Task<ApiResponseDto<ReferenceResponseDto>> CreateReferenceAsync(Guid contentItemId, CreateExternalReferenceDto createDto)
    {
        var now = DateTime.UtcNow;
        
        var reference = new ExternalReference
        {
            Id = Guid.NewGuid(),
            ParentType = 0, // ContentItem
            ParentId = contentItemId,
            Url = createDto.Url,
            Title = createDto.Title ?? Path.GetFileName(createDto.Url),
            Type = createDto.Type,
            IsActive = true
        };

        _context.ExternalReferences.Add(reference);
        await _context.SaveChangesAsync();

        return ApiResponseDto<ReferenceResponseDto>.Success(new ReferenceResponseDto());
    }
}