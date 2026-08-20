// =============================================================================
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================

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
    public async Task<ApiResponseDto<ReferenceListResponse>> GetReferencesAsync(Guid contentItemId)
    {
        var references = await _context.ExternalReferences
            .Where(r => r.ParentType == 0 && r.ParentId == contentItemId && r.IsActive)
            .ToListAsync();

        return ApiResponseDto.Success<ReferenceListResponse>(new ReferenceListResponse());
    }

    /// <summary>
    /// Create external reference.
    /// </summary>
    public async Task<ApiResponseDto<ReferenceResponse>> CreateReferenceAsync(Guid contentItemId, CreateExternalReferenceDto createDto)
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

        return ApiResponseDto.Success<ReferenceResponse>(new ReferenceResponse());
    }
}