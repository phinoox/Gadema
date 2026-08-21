// =============================================================================
using GameDev.Core.Dtos;
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using GameDev.Core.Dtos.Comments;
using GameDev.Core.Dtos.ContentItems;
using GameDev.Core.Dtos.Export;
using GameDev.Core.Dtos.Reviews;
using GameDev.Core.Dtos.Search;
using GameDev.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GameDev.Core.Dtos.DialogueTrees;
using GameDev.Core.Dtos.ExternalReferences;
using GameDev.Core.Dtos.Tags;

namespace GameDev.Api.Services;

/// <summary>
/// Interface for dialogue service.
/// </summary>
public interface IDialogueService
{
    Task<ApiResponseDto<BranchListResponseDto>> GetBranchesAsync(Guid projectId);
    Task<ApiResponseDto<BranchResponseDto>> CreateBranchAsync(Guid projectId, CreateBranchDto createDto);
}

/// <summary>
/// Interface for external reference service.
/// </summary>
public interface IExternalReferenceService
{
    Task<ApiResponseDto<ReferenceListResponseDto>> GetReferencesAsync(Guid contentItemId);
    Task<ApiResponseDto<ReferenceResponseDto>> CreateReferenceAsync(Guid contentItemId, CreateExternalReferenceDto createDto);
}

/// <summary>
/// Interface for task service.
/// </summary>
public interface ITaskService
{
    Task<ApiResponseDto<PaginationResponse<TaskResponseDto>>> GetTasksAsync(Guid? projectId, int? status, int? difficulty, bool isQuickWin);
    Task<ApiResponseDto<TaskResponseDto>> CreateTaskAsync(CreateTaskDto createDto);
    Task<ApiResponseDto<TaskResponseDto>> UpdateTaskAsync(Guid id, UpdateTaskDto updateDto);
    Task<ApiResponseDto<TaskResponseDto>> DeleteTaskAsync(Guid id);
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
    Task<ApiResponseDto<ReviewStatusResponseDto>> GetReviewStatusAsync(Guid contentItemId);
    Task<ApiResponseDto<ReviewStatusResponseDto>> ApproveContentAsync(Guid contentItemId, ApproveContentDto approveDto);
}

/// <summary>
/// Interface for comment service.
/// </summary>
public interface ICommentService
{
    Task<ApiResponseDto<CommentListResponseDto>> GetCommentsAsync(Guid contentItemId, string? visibility = null);
    Task<ApiResponseDto<CommentResponseDto>> CreateCommentAsync(Guid contentItemId, CreateCommentDto createDto);
}

/// <summary>
/// Interface for tag service.
/// </summary>
public interface ITagService
{
    Task<ApiResponseDto<TagListResponseDto>> GetTagsAsync(Guid contentItemId);
    Task<ApiResponseDto<TagListResponseDto>> AddTagsAsync(Guid contentItemId, AddTagsDto addDto);
}

/// <summary>
/// Interface for search service.
/// </summary>
public interface ISearchService
{
    Task<ApiResponseDto<PaginationResponse<ContentItemResponseDto>>> SearchContentItemsAsync(SearchContentItemsDto searchDto);
}