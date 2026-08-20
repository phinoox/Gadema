// =============================================================================
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================

namespace GameDev.Api.Services;

/// <summary>
/// Interface for dialogue service.
/// </summary>
public interface IDialogueService
{
    Task<ApiResponseDto<BranchListResponse>> GetBranchesAsync(Guid projectId);
    Task<ApiResponseDto<BranchResponse>> CreateBranchAsync(Guid projectId, CreateBranchDto createDto);
}

/// <summary>
/// Interface for external reference service.
/// </summary>
public interface IExternalReferenceService
{
    Task<ApiResponseDto<ReferenceListResponse>> GetReferencesAsync(Guid contentItemId);
    Task<ApiResponseDto<ReferenceResponse>> CreateReferenceAsync(Guid contentItemId, CreateExternalReferenceDto createDto);
}

/// <summary>
/// Interface for task service.
/// </summary>
public interface ITaskService
{
    Task<ApiResponseDto<PaginationResponse<TaskResponse>>> GetTasksAsync(Guid? projectId, int? status, int? difficulty, bool isQuickWin);
    Task<ApiResponseDto<TaskResponse>> CreateTaskAsync(CreateTaskDto createDto);
    Task<ApiResponseDto<TaskResponse>> UpdateTaskAsync(Guid id, UpdateTaskDto updateDto);
    Task<ApiResponseDto<object>> DeleteTaskAsync(Guid id);
}

/// <summary>
/// Interface for export service.
/// </summary>
public interface IExportService
{
    Task<IActionResult> ExportToJsonAsync(Guid projectId, ExportJsonDto exportDto);
    Task<IActionResult> ExportToCsvAsync(Guid projectId, ExportCsvDto exportDto);
    Task<IActionResult> ExportToXmlGddAsync(Guid projectId, ExportXmlGddDto exportDto);
    Task<IActionResult> ExportToPdfAsync(Guid projectId, ExportPdfDto exportDto);
}

/// <summary>
/// Interface for review service.
/// </summary>
public interface IReviewService
{
    Task<ApiResponseDto<ReviewStatusResponse>> GetReviewStatusAsync(Guid contentItemId);
    Task<ApiResponseDto<ReviewStatusResponse>> ApproveContentAsync(Guid contentItemId, ApproveContentDto approveDto);
}

/// <summary>
/// Interface for comment service.
/// </summary>
public interface ICommentService
{
    Task<ApiResponseDto<CommentListResponse>> GetCommentsAsync(Guid contentItemId, string? visibility = null);
    Task<ApiResponseDto<CommentResponse>> CreateCommentAsync(Guid contentItemId, CreateCommentDto createDto);
}

/// <summary>
/// Interface for tag service.
/// </summary>
public interface ITagService
{
    Task<ApiResponseDto<TagListResponse>> GetTagsAsync(Guid contentItemId);
    Task<ApiResponseDto<TagListResponse>> AddTagsAsync(Guid contentItemId, AddTagsDto addDto);
}

/// <summary>
/// Interface for search service.
/// </summary>
public interface ISearchService
{
    Task<ApiResponseDto<PaginationResponse<ContentItemResponseDto>>> SearchContentItemsAsync(SearchContentItemsDto searchDto);
}