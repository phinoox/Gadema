// =============================================================================
using Gadema.Core.Dtos;
// Gadema.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Gadema.Core.Dtos.Comments;
using Gadema.Core.Dtos.ContentItems;
using Gadema.Core.Dtos.Export;
using Gadema.Core.Dtos.Reviews;
using Gadema.Core.Dtos.Search;
using Gadema.Data.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Gadema.Core.Dtos.DialogueTrees;
using Gadema.Core.Dtos.ExternalReferences;
using Gadema.Core.Dtos.Tags;
using Gadema.Core.Dtos.Tasks;

namespace Gadema.Api.Services;

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
public interface IProjectTaskService
{
    Task<ApiResponseDto<PaginationResponse<ProjectTaskResponseDto>>> GetTasksAsync(Guid? projectId, int? status, int? difficulty, bool isQuickWin);
    Task<ApiResponseDto<ProjectTaskResponseDto>> CreateTaskAsync(ProjectTaskCreateDto createDto);
    Task<ApiResponseDto<ProjectTaskResponseDto>> UpdateTaskAsync(Guid id, ProjectTaskUpdateDto updateDto);
    Task<ApiResponseDto<ProjectTaskResponseDto>> DeleteTaskAsync(Guid id);
}

/// <summary>
/// Interface for export service.
/// </summary>
public interface IExportService
{
    Task<ExportJsonResponseDto> ExportToJsonAsync(Guid projectId, ExportJsonDto exportDto);
    Task<ExportCsvResponseDto> ExportToCsvAsync(Guid projectId, ExportCsvDto exportDto);
    Task<ExportXmlGddResponseDto> ExportToXmlGddAsync(Guid projectId, ExportXmlGddDto exportDto);
    Task<ExportPdfResponseDto> ExportToPdfAsync(Guid projectId, ExportPdfDto exportDto);
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