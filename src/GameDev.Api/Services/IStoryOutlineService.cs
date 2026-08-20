// =============================================================================
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================

namespace GameDev.Api.Services;

/// <summary>
/// Interface for story outline service.
/// </summary>
public interface IStoryOutlineService
{
    Task<ApiResponseDto<SequenceListResponse>> GetSequencesAsync(Guid projectId);
    Task<ApiResponseDto<SequenceResponse>> CreateSequenceAsync(Guid projectId, CreateSequenceDto createDto);
}