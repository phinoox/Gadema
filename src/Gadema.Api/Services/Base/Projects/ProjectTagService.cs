using Gadema.Api.CoreServices;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Core.Models.Base.Projects;
using Gadema.Data.Database.Core;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Base.Projects;

[ServiceLifetime(ServiceLifetime.Scoped)] public class ProjectTagService : DomainService
{
    private CoreDbContext _db;

    public ProjectTagService( CoreDbContext db,
        ILogger<ProjectService> logger,  
        CoreServicesProvider coreServices) // Injected via CoreService constructor
        : base(coreServices,logger)
    {
         _db = db;
    }

    public async Task<ApiResponseDto<ListResponseDto<ProjectTagResponseDto>>> GetTagsAsync(Guid projectId)
    {
        var error = await CheckAccessAsync<ListResponseDto<ProjectTagResponseDto>>(projectId, Permission.CanView);
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
        var error = await CheckAccessAsync<CreateResponseDto>(projectId, Permission.CanEdit);
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
        // Note: ProjectMetaInfo is part of the Project scope. 
        // We use the MetaInfo ID to check access against its parent project.
        // For now, we assume checking permission on the meta info directly via the engine if possible, 
        // or we need to resolve the project from the meta info first.
        
        var meta = await _db.Set<BaseMetaInfo>().OfType<ProjectMetaInfo>().FirstOrDefaultAsync(m => m.Id == projectMetaInfoId);
        if (meta == null) return ApiResponseDto<string>.NotFound("Project metadata not found.");

        var error = await CheckAccessAsync<string>(meta.ProjectId.Value, Permission.CanEdit);
        if (error != null) return error;

        foreach (var tagId in dto.TagIds)
        {
            var exists = await _db.ProjectTagRelations
                .AnyAsync(r => r.MetaInfoId == projectMetaInfoId && r.TagId == tagId);

            if (!exists)
            {
                _db.ProjectTagRelations.Add(new ProjectTagRelation
                {
                    MetaInfoId = projectMetaInfoId,
                    TagId = tagId
                });
            }
        }

        await _db.SaveChangesAsync();
        await LogDbAsync(meta.ProjectId.Value, "Updated", "Project", projectMetaInfoId, "Tags added to project.");

        return ApiResponseDto<string>.Success("Tags linked successfully.");
    }
}