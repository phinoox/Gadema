// =============================================================================
using GameDev.Core.Dtos;
using Microsoft.EntityFrameworkCore;
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using GameDev.Core.Dtos.StoryOutlining;
using GameDev.Core.Models;
using GameDev.Data;
using Microsoft.Extensions.Logging;
using GameDev.Core.Services;

namespace GameDev.Api.Services;

/// <summary>
/// Implementation of story outline service.
/// </summary>
public class StoryOutlineService : IGademaService,  IStoryOutlineService
{
    private readonly GameDbContext _context;
    private readonly ILogger<StoryOutlineService> _logger;

    public ServiceTypeEnum ServiceType => ServiceTypeEnum.StoryOutlineService;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public StoryOutlineService(GameDbContext context, ILogger<StoryOutlineService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// List story sequences for project.
    /// </summary>
    public async Task<ApiResponseDto<SequenceListResponseDto>> GetSequencesAsync(Guid projectId)
    {
        var sequences = await _context.StorySequences
            .Where(s => s.ProjectId == projectId)
            .OrderBy(s => s.OrderIndex)
            .Include(s => s.OutlineSummary)
            .ToListAsync();

        return ApiResponseDto<SequenceListResponseDto>.Success(new SequenceListResponseDto());
    }

    /// <summary>
    /// Create new sequence (chapter).
    /// </summary>
    public async Task<ApiResponseDto<SequenceResponseDto>> CreateSequenceAsync(Guid projectId, CreateSequenceDto createDto)
    {
        var now = DateTime.UtcNow;
        
        var sequence = new StorySequence
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            SequenceName = createDto.SequenceName,
            Slug = createDto.Slug ?? SlugHelper.GenerateSlug(createDto.SequenceName),
            SequenceDescription = null,
            Published = false,
            OrderIndex = 0
        };

        _context.StorySequences.Add(sequence);
        await _context.SaveChangesAsync();

        return ApiResponseDto<SequenceResponseDto>.Success(new SequenceResponseDto());
    }
}