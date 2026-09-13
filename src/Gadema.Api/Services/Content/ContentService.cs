using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Content;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Gadema.Core.Dtos.MetaInfos;

namespace Gadema.Api.Services.Content;

public class ContentService : CoreService
{
    public ContentService(GameDbContext db, ILogger<ContentService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // METAINFO CRUD (as expected by MetaInfoController)
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<MetaInfoResponseDto>>> GetMetaInfosAsync(Guid projectId, int page = 1, int pageSize = 20)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<MetaInfoResponseDto>>(projectId);
        if (error != null) return error;

        var query = _db.MetaInfos
            .Where(m => m.ProjectId == projectId)
            .OrderByDescending(m => m.LastModifiedAt);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(m => new MetaInfoResponseDto
            {
                Id = m.Id,
                ProjectId = m.ProjectId,
                ContentType = m.ContentType,
                Title = m.Title,
                Slug = m.Slug,
                ShortDesc = m.ShortDesc,
                Status = m.Status,
                IsPublic = m.IsPublic,
                ViewMode = m.ViewMode,
                Version = m.Version,
                CreatedAt = m.CreatedAt,
                LastModifiedAt = m.LastModifiedAt
            })
            .ToListAsync();

        return ApiResponseDto<ListResponseDto<MetaInfoResponseDto>>.Success(new ListResponseDto<MetaInfoResponseDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }

    public async Task<ApiResponseDto<MetaInfoResponseDto>> GetMetaInfoAsync(Guid id)
    {
        var metaInfo = await _db.MetaInfos.FirstOrDefaultAsync(m => m.Id == id);
        if (metaInfo == null) return ApiResponseDto<MetaInfoResponseDto>.NotFound("MetaInfo not found.");

        var error = await ValidateProjectAccessAsync<MetaInfoResponseDto>(metaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<MetaInfoResponseDto>.Success(new MetaInfoResponseDto
        {
            Id = metaInfo.Id,
            ProjectId = metaInfo.ProjectId,
            ContentType = metaInfo.ContentType,
            Title = metaInfo.Title,
            Slug = metaInfo.Slug,
            ShortDesc = metaInfo.ShortDesc,
            Status = metaInfo.Status,
            IsPublic = metaInfo.IsPublic,
            ViewMode = metaInfo.ViewMode,
            Version = metaInfo.Version,
            CreatedAt = metaInfo.CreatedAt,
            LastModifiedAt = metaInfo.LastModifiedAt
        });
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateMetaInfoAsync(Guid projectId, CreateMetaInfoDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var metaInfo = new MetaInfo
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            ContentType = createDto.ContentType,
            Title = createDto.Title,
            Slug = string.IsNullOrWhiteSpace(createDto.Slug) ? GenerateSlug(createDto.Title) : createDto.Slug,
            ShortDesc = createDto.ShortDesc,
            Status = createDto.Status,
            IsPublic = createDto.IsPublic,
            ViewMode = createDto.ViewMode,
            CreatedByUserId = _userContext.CurrentUser!.Id,
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow
        };

        _db.MetaInfos.Add(metaInfo);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = metaInfo.Id,
            ProjectId = projectId
        });
    }

    public async Task<ApiResponseDto<MetaInfoResponseDto>> UpdateMetaInfoAsync(Guid id, UpdateMetaInfoDto updateDto)
    {
        var metaInfo = await _db.MetaInfos.FirstOrDefaultAsync(m => m.Id == id);
        if (metaInfo == null) return ApiResponseDto<MetaInfoResponseDto>.NotFound("MetaInfo not found.");

        var error = await ValidateProjectAccessAsync<MetaInfoResponseDto>(metaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(metaInfo, updateDto);
        metaInfo.LastModifiedAt = DateTime.UtcNow;
        metaInfo.Version++;

        await _db.SaveChangesAsync();

        return ApiResponseDto<MetaInfoResponseDto>.Success(new MetaInfoResponseDto
        {
            Id = metaInfo.Id,
            ProjectId = metaInfo.ProjectId,
            ContentType = metaInfo.ContentType,
            Title = metaInfo.Title,
            Slug = metaInfo.Slug,
            ShortDesc = metaInfo.ShortDesc,
            Status = metaInfo.Status,
            IsPublic = metaInfo.IsPublic,
            ViewMode = metaInfo.ViewMode,
            Version = metaInfo.Version,
            CreatedAt = metaInfo.CreatedAt,
            LastModifiedAt = metaInfo.LastModifiedAt
        });
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteMetaInfoAsync(Guid id)
    {
        var metaInfo = await _db.MetaInfos.FirstOrDefaultAsync(m => m.Id == id);
        if (metaInfo == null) return ApiResponseDto<DeleteResponseDto>.NotFound("MetaInfo not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(metaInfo.ProjectId);
        if (error != null) return error;

        _db.MetaInfos.Remove(metaInfo);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = metaInfo.ProjectId
        });
    }

    // ========================================================================
    // AUTOSAVE & ROLLBACK
    // ========================================================================

    public async Task<ApiResponseDto<AutosaveResponseDto>> AutosaveAsync(Guid id, AutosaveDto autosaveDto)
    {
        var metaInfo = await _db.MetaInfos.FirstOrDefaultAsync(m => m.Id == id);
        if (metaInfo == null) return ApiResponseDto<AutosaveResponseDto>.NotFound("MetaInfo not found.");

        var error = await ValidateProjectAccessAsync<AutosaveResponseDto>(metaInfo.ProjectId);
        if (error != null) return error;

        // Create a version log entry for the autosave
        var versionLog = new ContentVersionLog
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id,
            VersionNumber = metaInfo.Version,
           // Content = autosaveDto.Content,
            CreatedByUserId = _userContext.CurrentUser!.Id,
            CreatedAt = DateTime.UtcNow
        };

        _db.ContentVersionLogs.Add(versionLog);
        metaInfo.LastModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponseDto<AutosaveResponseDto>.Success(new AutosaveResponseDto
        {
            VersionLogId = versionLog.Id,
            VersionNumber = metaInfo.Version
        });
    }

    public async Task<ApiResponseDto<MetaInfoResponseDto>> RollbackAsync(Guid id, RollbackDto rollbackDto)
    {
        var metaInfo = await _db.MetaInfos.FirstOrDefaultAsync(m => m.Id == id);
        if (metaInfo == null) return ApiResponseDto<MetaInfoResponseDto>.NotFound("MetaInfo not found.");

        var error = await ValidateProjectAccessAsync<MetaInfoResponseDto>(metaInfo.ProjectId);
        if (error != null) return error;

        var versionLog = await _db.ContentVersionLogs.FirstOrDefaultAsync(v => v.Id == rollbackDto.VersionLogId);
        if (versionLog == null) return ApiResponseDto<MetaInfoResponseDto>.NotFound("Version log not found.");

        // Apply the content from the version log
        metaInfo.Title = versionLog.Content; // Assuming content is stored in Title for simplicity or a separate field
        metaInfo.LastModifiedAt = DateTime.UtcNow;
        metaInfo.Version++;

        await _db.SaveChangesAsync();

        return ApiResponseDto<MetaInfoResponseDto>.Success(new MetaInfoResponseDto
        {
            Id = metaInfo.Id,
            ProjectId = metaInfo.ProjectId,
            ContentType = metaInfo.ContentType,
            Title = metaInfo.Title,
            Slug = metaInfo.Slug,
            ShortDesc = metaInfo.ShortDesc,
            Status = metaInfo.Status,
            IsPublic = metaInfo.IsPublic,
            ViewMode = metaInfo.ViewMode,
            Version = metaInfo.Version,
            CreatedAt = metaInfo.CreatedAt,
            LastModifiedAt = metaInfo.LastModifiedAt
        });
    }

    // ========================================================================
    // MEDIA UPLOAD
    // ========================================================================

    public async Task<ApiResponseDto<MediaAttachmentResponseDto>> UploadMediaAsync(Guid id, IFormFile file, string fileName)
    {
        var metaInfo = await _db.MetaInfos.FirstOrDefaultAsync(m => m.Id == id);
        if (metaInfo == null) return ApiResponseDto<MediaAttachmentResponseDto>.NotFound("MetaInfo not found.");

        var error = await ValidateProjectAccessAsync<MediaAttachmentResponseDto>(metaInfo.ProjectId);
        if (error != null) return error;

        var attachment = new MediaAttachment
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id,
            FileName = fileName,
            ContentType = file.ContentType,
            StoragePath = $"/uploads/{metaInfo.Id}/{fileName}",
            FileSize = file.Length,
            UploadedByUserId = _userContext.CurrentUser!.Id,
            UploadedAt = DateTime.UtcNow
        };

        // In a real app, save file to storage here
        // await file.CopyToAsync(stream);

        _db.MediaAttachments.Add(attachment);
        await _db.SaveChangesAsync();

        return ApiResponseDto<MediaAttachmentResponseDto>.Success(new MediaAttachmentResponseDto
        {
            Id = attachment.Id,
            FileName = attachment.FileName,
            ContentType = attachment.ContentType,
            FileSize = attachment.FileSize,
            StoragePath = attachment.StoragePath
        });
    }
}