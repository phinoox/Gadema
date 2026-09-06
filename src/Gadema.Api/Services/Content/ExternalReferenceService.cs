// =============================================================================
using Gadema.Core.Dtos;
using Microsoft.EntityFrameworkCore;
// Gadema.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gadema.Core.Dtos.ExternalReferences;
using Gadema.Core.Models;
using Gadema.Data.Database;
using Microsoft.Extensions.Logging;
using Gadema.Core.Services;

namespace Gadema.Api.Services;

/// <summary>
/// Implementation of external reference service.
/// </summary>
public class ExternalReferenceService : IGademaService,  IExternalReferenceService
{
    private readonly GameDbContext _context;
    private readonly ILogger<ExternalReferenceService> _logger;

    public ServiceTypeEnum ServiceType => ServiceTypeEnum.ExternalReferenceService;

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
    public async Task<ApiResponseDto<ReferenceListResponseDto>> GetReferencesAsync(Guid MetaInfoId)
    {
        var references = await _context.ExternalReferences
            .Where(r => r.ParentType == 0 && r.ParentId == MetaInfoId && r.IsActive)
            .ToListAsync();

        return ApiResponseDto<ReferenceListResponseDto>.Success(new ReferenceListResponseDto());
    }

    /// <summary>
    /// Create external reference.
    /// </summary>
    public async Task<ApiResponseDto<ReferenceResponseDto>> CreateReferenceAsync(Guid MetaInfoId, CreateExternalReferenceDto createDto)
    {
        var now = DateTime.UtcNow;
        
        var reference = new ExternalReference
        {
            Id = Guid.NewGuid(),
            ParentType = 0, // MetaInfo
            ParentId = MetaInfoId,
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