using Gadema.Api.Services.Search;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Projects;
using Gadema.Core.Dtos.Search;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Models.Base.MetaInfo;
using Gadema.Core.Models.Projects;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Projects;

public class ProjectSeriesService : CoreService, ISearchableProvider
{
    public ProjectSeriesService(GameDbContext db, ILogger<ProjectSeriesService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    public async Task<ApiResponseDto<IEnumerable<ProjectSeriesResponseDto>>> GetSeriesAsync()
    {
        var series = await _db.ProjectSeries.ToListAsync();

        var projected = series.Select(s => new ProjectSeriesResponseDto
        {
            Id = s.Id,
            Title = s.MetaInfo.Title,
            Slug = s.MetaInfo.Slug,
            Description = s.MetaInfo.Description
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
            Title = series.MetaInfo.Title,
            Slug = series.MetaInfo.Slug,
            Description = series.MetaInfo.Description
        });
    }

   public async Task<ApiResponseDto<CreateResponseDto>> CreateSeriesAsync(ProjectSeriesCreateDto createDto)
    {
        // 1. Create the MetaInfo anchor using the generic helper
        var meta = CreateMetaInfo<ProjectSeriesMetaInfo>(createDto.ContentMetaInfo, m => {
            // Title and Slug are handled by the base class logic inside CreateMetaInfo<T>
        });

        // 2. Create the ProjectSeries entity with only its own domain properties
        var series = new ProjectSeries
        {
            Id = meta.Id, // The anchor's ID becomes the primary key for the series
            // Title and Slug are NOT here; they live in the MetaInfo anchor
        };

        _db.ProjectSeries.Add(series);
        _db.Set<ProjectSeriesMetaInfo>().Add(meta);
        
        await _db.SaveChangesAsync();

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

        var meta = await _db.Set<ProjectSeriesMetaInfo>().FirstOrDefaultAsync(m => m.ProjectSeriesId == id);
        
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
        if (updateDto.Description != null) series.MetaInfo.Description = updateDto.Description;

        // 2. Update Identity via Strategy
        if (updateDto.ContentMetaInfo != null)
        {
            await ApplyIdentitySyncAsync(series.Id, updateDto.ContentMetaInfo, new ProjectSeriesIdentityStrategy(_db));
        }

        await _db.SaveChangesAsync();

        return ApiResponseDto<ProjectSeriesResponseDto>.Success(new ProjectSeriesResponseDto
        {
            Id = series.Id,
            Title = series.MetaInfo.Title,
            Slug = series.MetaInfo.Slug,
            Description = series.MetaInfo.Description
        });
    }

    public async Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId)
    {
        return await _db.ProjectSeries
            .Where(s => s.MetaInfo.Title.Contains(query) || s.MetaInfo.Slug.Contains(query))
            .Select(s => new SearchHitDto
            {
                ResourceId = s.Id,
                DisplayName = s.MetaInfo.Title,
                Slug = s.MetaInfo.Slug,
                ResourceType = "ProjectSeries",
                ScopeId = s.Id,
                ResourceLink = $"/api/v1/project-series/{s.Id}"
            })
            .ToListAsync();
    }
}