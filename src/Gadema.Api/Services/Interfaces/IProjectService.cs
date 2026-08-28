// =============================================================================
using Gadema.Core.Dtos;
// Gadema.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Gadema.Core.Dtos.Projects;
using Gadema.Data;
using Microsoft.Extensions.Logging;

namespace Gadema.Api.Services;

/// <summary>
/// Interface for project service.
/// </summary>
public interface IProjectService
{
    Task<ApiResponseDto<PaginationResponse<ProjectResponseDto>>> GetProjectsAsync(int page, int pageSize, string? status = null, int? visibility = null, string? search = null);
    Task<ApiResponseDto<ProjectResponseDto>> CreateProjectAsync(CreateProjectDto createDto);
    Task<ApiResponseDto<ProjectResponseDto>> UpdateProjectAsync(Guid id, UpdateProjectDto updateDto);
    Task<ApiResponseDto<object>> TransferOrDeleteProjectAsync(Guid id);
    Task<ApiResponseDto<ProjectTokenResponseDto>> CreateApiTokenAsync(Guid id, ProjectTokenDto tokenDto);
    Task<ApiResponseDto<TokenListResponseDto>> GetProjectTokensAsync(Guid id);
    Task<ApiResponseDto<object>> RevokeApiTokenAsync(Guid id, Guid tokenId);
}