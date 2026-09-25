using Gadema.Api.Services.Projects;
using Gadema.Api.Services.Search;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Dtos.Search;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Projects;
using Gadema.Data.Database;
using Gadema.Data.Database.Game;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Base.Projects;

[ServiceLifetime(ServiceLifetime.Scoped)] public class ProjectSeriesService : CoreService, ISearchableProvider
{
    public ProjectSeriesService(GameDbContext db, ILogger<ProjectSeriesService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

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
        // This avoids the circular dependency during the first SaveChanges.
        var meta = CreateMetaInfo<ProjectSeriesMetaInfo>(createDto.MetaInfo, m => {
            // Title and Slug are handled by the base class logic inside CreateMetaInfo<T>
        });
        _db.Set<ProjectSeriesMetaInfo>().Add(meta);
        await _db.SaveChangesAsync();
        // 2. Create the ProjectSeries entity (Body)
        var series = new ProjectSeries
        {
          Id = Guid.NewGuid(),
          ProjectSeriesMetaInfoId = meta.Id
        };

        // We add both to the context. 
        // Because meta.ProjectSeriesId is null, the FK constraint for MetaInfo is satisfied.
        _db.ProjectSeries.Add(series);
                
        await _db.SaveChangesAsync();

        // 3. Now that both exist in the DB, we can link them back together (The "Identity Sync")
        //meta.ProjectSeriesId = series.Id;
        

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = series.Id,
            MetaInfoId = meta.Id
        });
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteSeriesAsync(Guid id)
    {
        var series = await _db.ProjectSeries.FindAsync(id);
        if (series == null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Series with ID {id} not found.");

        var meta = await _db.Set<ProjectSeriesMetaInfo>().FirstOrDefaultAsync(m => m.Id == series.ProjectSeriesMetaInfoId);
        
        _db.ProjectSeries.Remove(series);
        if (meta != null) _db.Set<ProjectSeriesMetaInfo>().Remove(meta);

        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto { EntityId = id });
    }

    public async Task<ApiResponseDto<ProjectSeriesResponseDto>> UpdateSeriesAsync(Guid id, ProjectSeriesUpdateDto updateDto)
    {
        var series = await _db.ProjectSeries.FindAsync(id);
        if (series == null)
            return ApiResponseDto<ProjectSeriesResponseDto>.NotFound($"Series with ID {id} not found.");

        // 1. Update Domain properties
        if (updateDto.Description != null) series.ProjectSeriesMetaInfo.Description = updateDto.Description;

        // 2. Update Identity via Strategy
        if (updateDto.MetaInfo != null)
        {
            await ApplyIdentitySyncAsync(series.ProjectSeriesMetaInfoId, updateDto.MetaInfo, new ProjectSeriesIdentityStrategy(_db));
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