// =============================================================================
using GameDev.Core.Dtos;
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using GameDev.Core.Dtos.ContentItems;
using GameDev.Core.Enums;
using GameDev.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GameDev.Core.Dtos.Response;

namespace GameDev.Api.Services;

/// <summary>
/// Interface for content item service.
/// </summary>
public interface IContentService
{
    Task<ApiResponseDto<PaginationResponse<ContentItemResponseDto>>> GetContentItemsAsync(Guid? projectId, ContentTypeEnum? contentType, int? status, bool published, ViewModeEnum viewMode);
    Task<ApiResponseDto<ContentItemResponseDto>> GetContentItemAsync(Guid id, ViewModeEnum viewMode);
    Task<ApiResponseDto<ContentItemResponseDto>> CreateContentItemAsync(CreateContentItemDto createDto);
    Task<ApiResponseDto<ContentItemResponseDto>> UpdateContentItemAsync(Guid id, UpdateContentItemDto updateDto);
    Task<ApiResponseDto<SimpleResponseDto>> DeleteContentItemAsync(Guid id);
    Task<ApiResponseDto<MediaAttachmentResponseDto>> UploadMediaAsync(Guid id, IFormFile file);
    Task<ApiResponseDto<PaginationResponse<ContentItemResponseDto>>> AutosaveAsync(Guid contentItemId);
    Task<ApiResponseDto<VersionInfo>> RollbackAsync(Guid id, RollbackDto rollbackDto);
}