// =============================================================================
using Gadema.Core.Dtos;
using Microsoft.EntityFrameworkCore;
// Gadema.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Gadema.Core.Dtos.ContentItems;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Dtos.Response;
using Gadema.Data.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Gadema.Core.Services;

namespace Gadema.Api.Services;

/// <summary>
/// Implementation of content item service.
/// </summary>
public class ContentItemService : IGademaService,  IContentService
{
    private readonly GameDbContext _context;
    private readonly ILogger<ContentItemService> _logger;

    public ServiceTypeEnum ServiceType => ServiceTypeEnum.ContentService;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public ContentItemService(GameDbContext context, ILogger<ContentItemService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// List all content items (paginated).
    /// </summary>
    public async Task<ApiResponseDto<PaginationResponse<ContentItemResponseDto>>> GetContentItemsAsync(
        Guid? projectId,
        ContentTypeEnum? contentType,
        ContentStatusEnum? status,
        bool published,
        ViewModeEnum viewMode)
    {
        var query = _context.ContentItems.AsQueryable();
        
        if (projectId.HasValue)
        {
            query = query.Where(c => c.ProjectId == projectId.Value);
        }
        
        if (contentType.HasValue)
        {
            query = query.Where(c => c.ContentType == contentType.Value);
        }
        
        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }
        
        if (published)
        {
            query = query.Where(c => c.Published);
        }
        
        query = query.OrderByDescending(c => c.CreatedAt);
        
        var items = await query.Skip(0).Take(20).Select(c => new ContentItemResponseDto
        {
            Id = c.Id,
            ProjectId = c.ProjectId,
            ContentType = (int)c.ContentType,
            Title = c.Title,
            Slug = c.Slug,
            ShortDesc = c.ShortDesc,
            Description = viewMode == ViewModeEnum.PrivateWriting ? c.Description : null,
            Published = c.Published,
            Status = c.Status,
            ViewMode = viewMode.ToString(),
            Version = c.Version
        }).ToListAsync<ContentItemResponseDto>();
        
        return ApiResponseDto<PaginationResponse<ContentItemResponseDto>>.Success(new PaginationResponse<ContentItemResponseDto>());
    }

    /// <summary>
    /// Get content item by ID.
    /// </summary>
    public async Task<ApiResponseDto<ContentItemResponseDto>> GetContentItemAsync(Guid id, ViewModeEnum viewMode)
    {
        var item = await _context.ContentItems.FindAsync(id);
        
        if (item == null)
        {
            return ApiResponseDto<ContentItemResponseDto>.NotFound($"ContentItem with ID {id} not found");
        }
        
        var response = new ContentItemResponseDto
        {
            Id = item.Id,
            ProjectId = item.ProjectId,
            ContentType = (int)item.ContentType,
            Title = item.Title,
            Slug = item.Slug,
            ShortDesc = item.ShortDesc,
            Description = viewMode == ViewModeEnum.PrivateWriting ? item.Description : null,
            Published = item.Published,
            Status = item.Status,
            ViewMode = viewMode.ToString(),
            Version = item.Version
        };
        
        return ApiResponseDto<ContentItemResponseDto>.Success(response);
    }

    /// <summary>
    /// Create/edit content item.
    /// </summary>
    public async Task<ApiResponseDto<ContentItemResponseDto>> CreateContentItemAsync(CreateContentItemDto createDto)
    {
        var now = DateTime.UtcNow;
        
        var item = new ContentItem
        {
            Id = Guid.NewGuid(),
            ProjectId = createDto.ProjectId,
            ContentType = createDto.ContentType,
            Title = createDto.Title,
            Slug = createDto.Slug ?? SlugHelper.GenerateSlug(createDto.Title),
            ShortDesc = createDto.ShortDesc,
            Description = createDto.Description,
            Published = false,
            Status = ContentStatusEnum.Draft,
            ViewMode = ViewModeEnum.PrivateWriting,
            Version = 0,
            OrderIndex = 0,
            References = null,
            CreatedByUserId = UserHelper.GetUserId(),
            LastModifiedAt = now,
            CreatedAt = now
        };
        
        _context.ContentItems.Add(item);
        await _context.SaveChangesAsync();
        
        var response = new ContentItemResponseDto
        {
            Id = item.Id,
            ProjectId = item.ProjectId,
            ContentType = (int)item.ContentType,
            Title = item.Title,
            Slug = item.Slug,
            ShortDesc = item.ShortDesc,
            Description = item.Description,
            Published = item.Published,
            Status = item.Status,
            ViewMode = item.ViewMode.ToString(),
            Version = item.Version
        };
        
        return ApiResponseDto<ContentItemResponseDto>.Success(response);
    }

    /// <summary>
    /// Update content item.
    /// </summary>
    public async Task<ApiResponseDto<ContentItemResponseDto>> UpdateContentItemAsync(Guid id, UpdateContentItemDto updateDto)
    {
        var item = await _context.ContentItems.FindAsync(id);
        
        if (item == null)
        {
            return ApiResponseDto<ContentItemResponseDto>.NotFound($"ContentItem with ID {id} not found");
        }
        
        if (!string.IsNullOrWhiteSpace(updateDto.Description))
        {
            item.Description = updateDto.Description;
        }
        
        if (updateDto.Published.HasValue)
        {
            item.Published = updateDto.Published.Value;
            
            if (item.Published)
            {
                item.Status = ContentStatusEnum.Published;
            }
        }
        
        if (updateDto.ViewMode.HasValue)
        {
            item.ViewMode = updateDto.ViewMode.Value;
        }
        
        item.LastModifiedAt = DateTime.UtcNow;
        _context.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        await _context.SaveChangesAsync();
        
        var response = new ContentItemResponseDto
        {
            Id = item.Id,
            ProjectId = item.ProjectId,
            ContentType = (int)item.ContentType,
            Title = item.Title,
            Slug = item.Slug,
            ShortDesc = item.ShortDesc,
            Description = updateDto.ViewMode?.ToString() == "Presentation" ? null : item.Description,
            Published = item.Published,
            Status = item.Status,
            ViewMode = item.ViewMode.ToString(),
            Version = item.Version + 1
        };
        
        return ApiResponseDto<ContentItemResponseDto>.Success(response);
    }

    /// <summary>
    /// Delete content item.
    /// </summary>
    public async Task<ApiResponseDto<SimpleResponseDto>> DeleteContentItemAsync(Guid id)
    {
        var item = await _context.ContentItems.FindAsync(id);
        
        if (item == null)
        {
            return ApiResponseDto<SimpleResponseDto>.NotFound($"ContentItem with ID {id} not found");
        }
        
        _context.ContentItems.Remove(item);
        await _context.SaveChangesAsync();
        
        return ApiResponseDto<SimpleResponseDto>.Success(new SimpleResponseDto(){ Success = true, Message = "Content item has been deleted successfully" });
    }

    /// <summary>
    /// Upload media file to content item.
    /// </summary>
    public async Task<ApiResponseDto<MediaAttachmentResponseDto>> UploadMediaAsync(Guid id, IFormFile file)
    {
        var item = await _context.ContentItems.FindAsync(id);
        
        if (item == null)
        {
            return ApiResponseDto<MediaAttachmentResponseDto>.NotFound($"ContentItem with ID {id} not found");
        }
        
        if (file.Length > FileConfigurationConstants.MaxFileSize)
        {
            return ApiResponseDto<MediaAttachmentResponseDto>.BadRequest("File size exceeds maximum allowed (100MB)");
        }
        
        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var storagePath = $"uploads/{DateTime.Now:yyyy/MM}/{uniqueFileName}";
        
        // In production, use file stream to upload directly to disk or blob storage
        using var stream = new FileStream($"../{storagePath}", FileMode.Create);
        await file.CopyToAsync(stream);
        
        var attachment = new MediaAttachment
        {
            Id = Guid.NewGuid(),
            ContentItemId = id,
            FileName = uniqueFileName,
            ContentType = file.ContentType ?? "application/octet-stream",
            StoragePath = storagePath,
            FileSize = file.Length,
            UploadedByUserId = UserHelper.GetUserId(),
            UploadedAt = DateTime.UtcNow
        };
        
        _context.MediaAttachments.Add(attachment);
        await _context.SaveChangesAsync();
        
        return ApiResponseDto<MediaAttachmentResponseDto>.Success(new MediaAttachmentResponseDto
        {
            Id = attachment.Id,
            FileName = attachment.FileName,
            ContentType = attachment.ContentType,
            StoragePath = attachment.StoragePath,
            UploadedAt = attachment.UploadedAt
        });
    }

    /// <summary>
    /// Auto-save content item snapshot.
    /// </summary>
    public async Task<ApiResponseDto<SimpleResponseDto>> AutosaveAsync(Guid id)
    {
        var item = await _context.ContentItems.FindAsync(id);
        
        if (item == null)
        {
            return ApiResponseDto<SimpleResponseDto>.NotFound($"ContentItem with ID {id} not found");
        }
        
        // Create snapshot for version control
        var snapshot = new ContentSnapshot
        {
            Id = Guid.NewGuid(),
            ContentItemId = id,
            SnapshotVersion = item.Version + 1,
            SnapshotType = 0, // AutoGenerated
            SnapshotDataJson = JsonSerializer.Serialize(item),
            CreatedByUserId = UserHelper.GetUserId(),
            CreatedAt = DateTime.UtcNow
        };
        
        _context.ContentSnapshots.Add(snapshot);
        await _context.SaveChangesAsync();
        
        item.Version += 1;
        item.LastModifiedAt = DateTime.UtcNow;
        _context.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        await _context.SaveChangesAsync();
        
        return ApiResponseDto<SimpleResponseDto>.Success(new SimpleResponseDto(){Success = true, Message = "Saved Sucessfully"});
    }

    /// <summary>
    /// Rollback to previous version.
    /// </summary>
    public async Task<ApiResponseDto<VersionInfo>> RollbackAsync(Guid id, RollbackDto rollbackDto)
    {
        var item = await _context.ContentItems.FindAsync(id);
        
        if (item == null)
        {
            return ApiResponseDto<VersionInfo>.NotFound($"ContentItem with ID {id} not found");
        }
        
        // Find the target snapshot
        var snapshot = await _context.ContentSnapshots
            .FirstOrDefaultAsync(s => s.ContentItemId == id && s.SnapshotVersion <= rollbackDto.TargetVersion);
        
        if (snapshot == null)
        {
            return ApiResponseDto<VersionInfo>.BadRequest($"No snapshot found for version {rollbackDto.TargetVersion}");
        }
        
        // Restore from snapshot data
        var restoredData = JsonSerializer.Deserialize<ContentItem>(snapshot.SnapshotDataJson!);
        if (restoredData == null)
        {
            return ApiResponseDto<VersionInfo>.BadRequest($"Could not load restored data for version {rollbackDto.TargetVersion}");
        }
        var oldVersion = item.Version;
        item.Title = restoredData.Title;
        item.Description = restoredData.Description;
        item.ShortDesc = restoredData.ShortDesc;
        item.Slug = restoredData.Slug;
        item.Published = restoredData.Published;
        item.Status = restoredData.Status;
        item.ViewMode = restoredData.ViewMode;
        
        item.Version = rollbackDto.TargetVersion;
        item.LastModifiedAt = DateTime.UtcNow;
        _context.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        await _context.SaveChangesAsync();
        
        // Create new snapshot for the rollback point
        var newSnapshot = new ContentSnapshot
        {
            Id = Guid.NewGuid(),
            ContentItemId = id,
            SnapshotVersion = item.Version,
            SnapshotType = 2, // RollbackPoint
            SnapshotDataJson = JsonSerializer.Serialize(item),
            CreatedByUserId = UserHelper.GetUserId(),
            CreatedAt = DateTime.UtcNow
        };
        
        _context.ContentSnapshots.Add(newSnapshot);
        await _context.SaveChangesAsync();
        
        return ApiResponseDto<VersionInfo>.Success(new VersionInfo(){ Success = true,FromVersion = oldVersion,ToVersion = rollbackDto.TargetVersion,Message = "Content Item has been deleted successfully" });
    }

}