using Gadema.Api.CoreServices;
using Gadema.Api.CoreServices.Strategies;
using Gadema.Api.Services.Search;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Dtos.Search;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Core.Models.Base.Projects;
using Gadema.Data.Database.Core;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Base.Projects;

[ServiceLifetime(ServiceLifetime.Scoped)] 
public class ProjectSeriesService : DomainService, ISearchableProvider
{
    private CoreDbContext _db;

    public ProjectSeriesService(CoreDbContext db,
        ILogger<ProjectSeriesService> logger,  
        ICoreServicesProvider coreServices) // Injected via CoreService constructor
        : base(coreServices,logger)
    {
         _db = db;
    }
    public async Task<ApiResponseDto<IEnumerable<ProjectSeriesResponseDto>>> GetSeriesAsync()
    {
        var series = await _db.ProjectSeries.ToListAsync();

        var projected = series.Select(s => new ProjectSeriesResponseDto
        {
            Id = s.Id,
            Title = s.ProjectSeriesMetaInfo.Title,
            Slug = s.ProjectSeriesMetaInfo.Slug,
            Description = s.ProjectSeriesMetaInfo.Description
        }).ToList();

        return ApiResponseDto<IEnumerable<ProjectSeriesResponseDto>>.Success(projected);
    }

    public async Task<ApiResponseDto<ProjectSeriesResponseDto>> GetSeriesAsync(Guid id)
    {
        var error = await CheckAccessAsync<ProjectSeriesResponseDto>(id, Permission.CanView);
        if (error != null) return error;

        var series = await _db.ProjectSeries.FindAsync(id);

        if (series == null)
            return ApiResponseDto<ProjectSeriesResponseDto>.NotFound($"Series with ID {id} not found.");

        return ApiResponseDto<ProjectSeriesResponseDto>.Success(new ProjectSeriesResponseDto
        {
            Id = series.Id,
            Title = series.ProjectSeriesMetaInfo.Title,
            Slug = series.ProjectSeriesMetaInfo.Slug,
            Description = series.ProjectSeriesMetaInfo.Description
        });
    }

   public async Task<ApiResponseDto<CreateResponseDto>> CreateSeriesAsync(ProjectSeriesCreateDto createDto)
    {
        // 1. Create the MetaInfo anchor (Soul) first, but WITHOUT the ProjectSeriesId yet.
        var meta = await _core.MetadataService.CreateAsync<ProjectSeriesMetaInfo>(createDto.MetaInfo, m => { });
        _db.Set<ProjectSeriesMetaInfo>().Add(meta);
        await _db.SaveChangesAsync();

        // 2. Create the ProjectSeries entity (Body)
        var series = new ProjectSeries
        {
          Id = meta.Id
        };

        _db.ProjectSeries.Add(series);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = series.Id,
            MetaInfoId = meta.Id
        });
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteSeriesAsync(Guid id)
    {
        var error = await CheckAccessAsync<DeleteResponseDto>(id, Permission.CanDelete);
        if (error != null) return error;

        var series = await _db.ProjectSeries.FindAsync(id);
        if (series == null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Series with ID {id} not found.");

        var meta = await _db.Set<ProjectSeriesMetaInfo>().FirstOrDefaultAsync(m => m.Id == series.Id);
        
        _db.ProjectSeries.Remove(series);
        if (meta != null) _db.Set<ProjectSeriesMetaInfo>().Remove(meta);

        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto { EntityId = id });
    }

    public async Task<ApiResponseDto<ProjectSeriesResponseDto>> UpdateSeriesAsync(Guid id, ProjectSeriesUpdateDto updateDto)
    {
        var error = await CheckAccessAsync<ProjectSeriesResponseDto>(id, Permission.CanEdit);
        if (error != null) return error;

        var series = await _db.ProjectSeries.FindAsync(id);
        if (series == null)
            return ApiResponseDto<ProjectSeriesResponseDto>.NotFound($"Series with ID {id} not found.");

        // 1. Update Domain properties
        if (updateDto.Description != null) series.ProjectSeriesMetaInfo.Description = updateDto.Description;

        // 2. Sync Identity via Strategy using the new DomainService method
        if (updateDto.MetaInfo != null)
        {
            var success = await SyncIdentityAsync<ProjectSeriesIdentityStrategy>(series.Id, updateDto.MetaInfo);
            if (!success) return ApiResponseDto<ProjectSeriesResponseDto>.ServerError("Sync failed.");
        }

        await _db.SaveChangesAsync();

        return ApiResponseDto<ProjectSeriesResponseDto>.Success(new ProjectSeriesResponseDto
        {
            Id = series.Id,
            Title = series.ProjectSeriesMetaInfo.Title,
            Slug = series.ProjectSeriesMetaInfo.Slug,
            Description = series.ProjectSeriesMetaInfo.Description
        });
    }

    public async Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId)
    {
        return await _db.ProjectSeries
            .Where(s => s.ProjectSeriesMetaInfo.Title.Contains(query) || s.ProjectSeriesMetaInfo.Slug.Contains(query))
            .Select(s => new SearchHitDto
            {
                ResourceId = s.Id,
                DisplayName = s.ProjectSeriesMetaInfo.Title,
                Slug = s.ProjectSeriesMetaInfo.Slug,
                ResourceType = "ProjectSeries",
                ScopeId = s.Id,
                ResourceLink = $"/api/v1/project-series/{s.Id}"
            })
            .ToListAsync();
    }
}