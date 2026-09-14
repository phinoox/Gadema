using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Projects;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Models.Projects;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Projects;

public class ProjectTagService : CoreService
{
    public ProjectTagService(GameDbContext db, ILogger<ProjectTagService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    public async Task<ApiResponseDto<ListResponseDto<ProjectTagResponseDto>>> GetTagsAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<ProjectTagResponseDto>>(projectId);
        if (error != null) return error;

        var tags = await _db.ProjectTags
            .Select(t => new ProjectTagResponseDto
            {
                Id = t.Id,
                Name = t.Name,
                Slug = t.Slug,
                ColorHex = t.ColorHex
            })
            .ToListAsync();

        return ApiResponseDto<ListResponseDto<ProjectTagResponseDto>>.Success(new ListResponseDto<ProjectTagResponseDto>
        {
            Items = tags,
            TotalCount = tags.Count
        });
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateTagAsync(Guid projectId, ProjectTagCreateDto dto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var tag = new ProjectTag
        {
            Name = dto.Name,
            Slug = dto.Slug ?? dto.Name.ToLower().Replace(" ", "-"),
            ColorHex = dto.ColorHex
        };

        _db.ProjectTags.Add(tag);
        await _db.SaveChangesAsync();

        await LogDbAsync(projectId, "Created", "ProjectTag", tag.Id, $"Tag '{tag.Name}' created.");

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto { EntityId = tag.Id });
    }

    public async Task<ApiResponseDto<string>> AddTagsToProjectAsync(Guid projectId, AddTagsToProjectDto dto)
    {
        var error = await ValidateProjectAccessAsync<string>(projectId);
        if (error != null) return error;

        foreach (var tagId in dto.TagIds)
        {
            var exists = await _db.ProjectTagRelations
                .AnyAsync(r => r.ProjectId == projectId && r.ProjectTagId == tagId);

            if (!exists)
            {
                _db.ProjectTagRelations.Add(new ProjectTagRelation
                {
                    ProjectId = projectId,
                    ProjectTagId = tagId
                });
            }
        }

        await _db.SaveChangesAsync();
        await LogDbAsync(projectId, "Updated", "Project", projectId, "Tags added to project.");

        return ApiResponseDto<string>.Success("Tags linked successfully.");
    }
}