// =============================================================================
using GameDev.Core.Dtos;
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using GameDev.Core.Dtos.StoryOutlining;
using GameDev.Data;
using Microsoft.Extensions.Logging;

namespace GameDev.Api.Services;

/// <summary>
/// Interface for story outline service.
/// </summary>
public interface IStoryOutlineService
{
    Task<ApiResponseDto<SequenceListResponseDto>> GetSequencesAsync(Guid projectId);
    Task<ApiResponseDto<SequenceResponseDto>> CreateSequenceAsync(Guid projectId, CreateSequenceDto createDto);
}