// =============================================================================
using Gadema.Core.Dtos;
// Gadema.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Gadema.Core.Dtos.MetaInfos;
using Gadema.Core.Enums;
using Gadema.Data.Database;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Gadema.Core.Dtos.Response;

namespace Gadema.Api.Services;

/// <summary>
/// Interface for content item service.
/// </summary>
public interface IContentService
{
    Task<ApiResponseDto<PaginationResponse<MetaInfoResponseDto>>> GetMetaInfosAsync(Guid? projectId, ContentTypeEnum? contentType, ContentStatusEnum? status, bool published, ViewModeEnum viewMode);
    Task<ApiResponseDto<MetaInfoResponseDto>> GetMetaInfoAsync(Guid id, ViewModeEnum viewMode);
    Task<ApiResponseDto<MetaInfoResponseDto>> CreateMetaInfoAsync(CreateMetaInfoDto createDto);
    Task<ApiResponseDto<MetaInfoResponseDto>> UpdateMetaInfoAsync(Guid id, UpdateMetaInfoDto updateDto);
    Task<ApiResponseDto<SimpleResponseDto>> DeleteMetaInfoAsync(Guid id);
    Task<ApiResponseDto<MediaAttachmentResponseDto>> UploadMediaAsync(Guid id, IFormFile file);
    Task<ApiResponseDto<SimpleResponseDto>> AutosaveAsync(Guid MetaInfoId);
    Task<ApiResponseDto<VersionInfo>> RollbackAsync(Guid id, RollbackDto rollbackDto);
}