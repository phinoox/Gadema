// =============================================================================
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================

namespace GameDev.Api.Services;

/// <summary>
/// Implementation of story outline service.
/// </summary>
public class StoryOutlineService : IStoryOutlineService
{
    private readonly GameDbContext _context;
    private readonly ILogger<StoryOutlineService> _logger;

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
    public async Task<ApiResponseDto<SequenceListResponse>> GetSequencesAsync(Guid projectId)
    {
        var sequences = await _context.StorySequences
            .Where(s => s.ProjectId == projectId)
            .OrderBy(s => s.OrderIndex)
            .Include(s => s.OutlineSummary)
            .ToListAsync();

        return ApiResponseDto.Success<SequenceListResponse>(new SequenceListResponse());
    }

    /// <summary>
    /// Create new sequence (chapter).
    /// </summary>
    public async Task<ApiResponseDto<SequenceResponse>> CreateSequenceAsync(Guid projectId, CreateSequenceDto createDto)
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

        return ApiResponseDto.Success<SequenceResponse>(new SequenceResponse());
    }
}