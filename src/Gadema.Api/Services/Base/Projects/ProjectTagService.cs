using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Projects;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Base.Projects;

[ServiceLifetime(ServiceLifetime.Scoped)] public class ProjectTagService : CoreService
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

    public async Task<ApiResponseDto<string>> AddTagsToProjectAsync(Guid projectMetaInfoId, AddTagsToProjectDto dto)
    {
        var error = await ValidateProjectAccessAsync<string>(projectMetaInfoId);
        if (error != null) return error;

        foreach (var tagId in dto.TagIds)
        {
            var exists = await _db.ProjectTagRelations
                .AnyAsync(r => r.ProjectMetaInfoId == projectMetaInfoId && r.ProjectMetaInfoId == tagId);

            if (!exists)
            {
                _db.ProjectTagRelations.Add(new ProjectTagRelation
                {
                    ProjectMetaInfoId = projectMetaInfoId,
                    TagId = tagId
                });
            }
        }

        await _db.SaveChangesAsync();
        await LogDbAsync(projectMetaInfoId, "Updated", "Project", projectMetaInfoId, "Tags added to project.");

        return ApiResponseDto<string>.Success("Tags linked successfully.");
    }
}