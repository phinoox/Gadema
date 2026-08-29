// =============================================================================
using Gadema.Core.Dtos;
// Gadema.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Gadema.Core.Dtos.StoryOutlining;
using Gadema.Core.Database;
using Microsoft.Extensions.Logging;

namespace Gadema.Api.Services;

/// <summary>
/// Interface for story outline service.
/// </summary>
public interface IStoryOutlineService
{
    Task<ApiResponseDto<SequenceListResponseDto>> GetSequencesAsync(Guid projectId);
    Task<ApiResponseDto<SequenceResponseDto>> CreateSequenceAsync(Guid projectId, CreateSequenceDto createDto);
}