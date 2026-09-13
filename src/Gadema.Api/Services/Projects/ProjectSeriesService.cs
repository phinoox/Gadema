// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Projects;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Enums;
using Gadema.Core.Models.Projects;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Projects;

/// <summary>
/// Service for managing ProjectSeries - organizational series within a project.
/// </summary>
public class ProjectSeriesService : CoreService
{
    public ProjectSeriesService(GameDbContext db, ILogger<ProjectSeriesService> logger, IUserContext userContext)
            : base(db, logger, userContext) { }

    private ProjectSeriesResponseDto CreateResponseDto(ProjectSeries series)
        => new()
        {
            Id = series.Id,
            MetaInfoId = series.MetaInfoId,
            MetaInfoTitle = series.MetaInfo.Title,
            SeriesName = series.SeriesName,
            Description = series.Description,
            OrderIndex = series.OrderIndex,
            IsPublished = series.IsPublished,
            CreatedAt = series.CreatedAt,
        };

    private ListResponseDto<ProjectSeriesResponseDto> CreateListResponseDto(IEnumerable<ProjectSeries> series)
        => new() { Items = series.Select(CreateResponseDto).ToList(), TotalCount = series.Count() };

    public async Task<ApiResponseDto<ListResponseDto<ProjectSeriesResponseDto>>> GetSeriesAsync(Guid projectId, int? orderIndex = null)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<ProjectSeriesResponseDto>>(projectId);
        if (error != null) return error;

        IQueryable<ProjectSeries> query = _db.ProjectSeries.Where(ps => ps.MetaInfo.ProjectId == projectId).OrderBy(ps => ps.OrderIndex);

        if (orderIndex.HasValue) query = query.Where(ps => ps.OrderIndex == orderIndex.Value);

        var series = await query.ToListAsync();
        return ApiResponseDto<ListResponseDto<ProjectSeriesResponseDto>>.Success(CreateListResponseDto(series));
    }

    public async Task<ApiResponseDto<ProjectSeriesResponseDto>> GetSeriesAsync(Guid id)
    {
        var series = await _db.ProjectSeries.Include(ps => ps.MetaInfo).FirstOrDefaultAsync(ps => ps.Id == id);
        if (series is null) return ApiResponseDto<ProjectSeriesResponseDto>.NotFound($"Project series with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<ProjectSeriesResponseDto>(series.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<ProjectSeriesResponseDto>.Success(CreateResponseDto(series));
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateSeriesAsync(Guid projectId, ProjectSeriesCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var metaInfo = CreateMetaInfo(projectId, ContentTypeEnum.ProjectSeries, createDto.CreateData);

        _db.MetaInfos.Add(metaInfo);
        await _db.SaveChangesAsync();

        // Auto-increment order index
        var maxIndex = await _db.ProjectSeries.Where(ps => ps.MetaInfo.ProjectId == projectId).MaxAsync(ps => ps.OrderIndex) ?? 0;

        var series = new ProjectSeries
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id.Value,
            SeriesName = createDto.SeriesName,
            Description = createDto.Description,
            OrderIndex = maxIndex + 1,
            IsPublished = false,
        };

        _db.ProjectSeries.Add(series);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto { EntityId = series.Id, MetaInfoId = metaInfo.Id.Value, ProjectId = projectId });
    }

    public async Task<ApiResponseDto<ProjectSeriesResponseDto>> UpdateSeriesAsync(Guid id, ProjectSeriesUpdateDto updateDto)
    {
        var series = await _db.ProjectSeries.Include(ps => ps.MetaInfo).FirstOrDefaultAsync(ps => ps.Id == id);
        if (series is null) return ApiResponseDto<ProjectSeriesResponseDto>.NotFound($"Project series with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<ProjectSeriesUpdateDto>(series.MetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(series.MetaInfo, updateDto.MetaInfo);

        if (!string.IsNullOrWhiteSpace(updateDto.SeriesName)) series.SeriesName = updateDto.SeriesName;
        if (updateDto.Description != null) series.Description = updateDto.Description;
        if (updateDto.OrderIndex.HasValue) series.OrderIndex = updateDto.OrderIndex.Value;
        if (updateDto.IsPublished.HasValue) series.IsPublished = updateDto.IsPublished.Value;

        await _db.SaveChangesAsync();
        return ApiResponseDto<ProjectSeriesResponseDto>.Success(CreateResponseDto(series));
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteSeriesAsync(Guid id)
    {
        var series = await _db.ProjectSeries.Include(ps => ps.MetaInfo).FirstOrDefaultAsync(ps => ps.Id == id);
        if (series is null) return ApiResponseDto<DeleteResponseDto>.NotFound($"Project series with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(series.MetaInfo.ProjectId);
        if (error != null) return error;

        _db.MetaInfos.Remove(series.MetaInfo);
        _db.ProjectSeries.Remove(series);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto { EntityId = id, ProjectId = series.MetaInfo.ProjectId });
    }
}