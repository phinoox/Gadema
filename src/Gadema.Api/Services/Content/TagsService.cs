using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Tags;
using Gadema.Core.Models;
using Gadema.Core.Models.Projects;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Content;

public class TagsService : CoreService
{
    public TagsService(GameDbContext db, ILogger<TagsService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    /// <summary>
    /// Alias for GetAllTagsAsync to satisfy the controller.
    /// </summary>
    public async Task<ApiResponseDto<ListResponseDto<TagResponseDto>>> GetTagsAsync(
        int page = 1,
        int pageSize = 20)
    {
        return await GetAllTagsAsync(page, pageSize);
    }

    /// <summary>
    /// Alias for AddTagsToProjectAsync to satisfy the controller.
    /// </summary>
    public async Task<ApiResponseDto<string>> AddTagsAsync(Guid projectId, AddTagsToProjectDto dto)
    {
        var error = await ValidateProjectAccessAsync<string>(projectId);
        if (error != null) return error;

        foreach (var tagId in dto.TagIds)
        {
            var tag = await _db.ProjectTags.FirstOrDefaultAsync(t => t.Id == tagId);
            if (tag == null) continue;

            var existing = await _db.ProjectTagRelations
                .FirstOrDefaultAsync(r => r.ProjectId == projectId && r.ProjectTagId == tagId);

            if (existing == null)
            {
                _db.ProjectTagRelations.Add(new ProjectTagRelation
                {
                    ProjectId = projectId,
                    ProjectTagId = tagId
                });
            }
        }

        await _db.SaveChangesAsync();
        return ApiResponseDto<string>.Success("Tags added successfully.");
    }

    // ... existing methods (GetAllTagsAsync, CreateTagAsync, etc.) ...
}

// DTO for the controller's AddTagsAsync endpoint
public class AddTagsToProjectDto
{
    public List<Guid> TagIds { get; set; } = new();
}