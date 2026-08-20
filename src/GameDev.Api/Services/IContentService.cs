// =============================================================================
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================

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
    Task<ApiResponseDto<object>> DeleteContentItemAsync(Guid id);
    Task<ApiResponseDto<MediaAttachmentResponseDto>> UploadMediaAsync(Guid id, IFormFile file);
    Task<ApiResponseDto<object>> AutosaveAsync(Guid id);
    Task<ApiResponseDto<object>> RollbackAsync(Guid id, RollbackDto rollbackDto);
}