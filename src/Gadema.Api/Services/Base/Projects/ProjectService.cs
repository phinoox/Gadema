using Gadema.Api.CoreServices;
using Gadema.Api.CoreServices.Strategies;
using Gadema.Api.Services.Search;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Dtos.Search;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Access;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Core.Models.Base.Projects;
using Gadema.Data.Database;
using Gadema.Data.Database.Core;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Base.Projects;

[ServiceLifetime(ServiceLifetime.Scoped)] 
public class ProjectService : DomainService, ISearchableProvider
{
    private CoreDbContext _db;

    public ProjectService(
        CoreDbContext _db,
        ILogger<ProjectService> logger,  
        CoreServicesProvider coreServices)
        : base(coreServices,logger)
    {
        _db = _db; // Corrected assignment
    }

    public async Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId)
    {
        var metaQuery = _db.MetaInfos.AsQueryable();

        if (projectId.HasValue)
        {
            metaQuery = metaQuery.Where(m => m.ProjectId == projectId.Value);
        }

        return await metaQuery
            .Where(m => m.Title.Contains(query) || m.Slug.Contains(query))
            .Select(m => new SearchHitDto
            {
                ResourceId = m.Id, 
                DisplayName = m.Title,
                Slug = m.Slug,
                ResourceType = "Project",
                ScopeId = null, 
                ResourceLink = $"/api/v1/projects/{m.Id}"
            })
            .ToListAsync();
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateAsync(ProjectCreateDto createDto)
    {
        var loggedIn = await CheckIsLoggedIn();
        if(!loggedIn)
            return ApiResponseDto<CreateResponseDto>.Unauthorized("you need to be logged in");

        // Law I: The Soul (MetaInfo) and the Body (Project) must share the same ID.
        
        // Note: In a real implementation, we'd ensure MetadataService supports passing a pre-defined ID 
        // or handle the synchronization immediately. For now, following the pattern of creating them together.
        var meta = await _core.MetadataService.CreateAsync<ProjectSeriesMetaInfo>(createDto.MetaInfo, m => {
        });

        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var project = new Project
            {
                Id = meta.Id, // LAW I: Body.Id == Soul.Id
                UserId = _userId,
                Description = createDto.Description,
                IsActive = true,
                EnableUserRegistration = createDto.EnableUserRegistration,
                AllowManualInvites = createDto.AllowManualInvites,
                PrimaryFormat = createDto.PrimaryFormat,
                Genre = createDto.Genre,
                Theme = createDto.Theme,
                Tone = createDto.Tone,
                Audience = createDto.Audience
            };

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto 
            { 
                EntityId = project.Id, 
                ProjectId = project.Id 
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger?.LogError(ex, "Error creating project");
            return ApiResponseDto<CreateResponseDto>.ServerError("Creation failed.");
        }
    }

    public async Task<ApiResponseDto<ProjectResponseDto>> GetAsync(Guid projectId, Guid contextProjectId)
    {
        var authorizationError = await CheckAccessAsync<ProjectResponseDto>(contextProjectId, Permission.CanView);
        if (authorizationError != null) 
            return authorizationError;

        var project = await _db.Projects
            .Include(p => p.ProjectMetaInfo)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null) return ApiResponseDto<ProjectResponseDto>.NotFound("Project not found.");

        return ApiResponseDto<ProjectResponseDto>.Success(MapToResponseDto(project));
    }

    public async Task<ApiResponseDto<ProjectResponseDto>> UpdateAsync(Guid projectId, Guid contextProjectId, ProjectUpdateDto dto)
    {
         var authorizationError = await CheckAccessAsync<ProjectResponseDto>(contextProjectId, Permission.CanView);
        if (authorizationError != null) 
            return authorizationError;

        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var project = await _db.Projects
                .Include(p => p.ProjectMetaInfo)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null) return ApiResponseDto<ProjectResponseDto>.NotFound("Project not found.");

            if (dto.Description != null) project.Description = dto.Description;
            if (dto.EnableUserRegistration.HasValue) project.EnableUserRegistration = dto.EnableUserRegistration.Value;
            if (dto.AllowManualInvites.HasValue) project.AllowManualInvites = dto.AllowManualInvites.Value;
            if (dto.PrimaryFormat.HasValue) project.PrimaryFormat = dto.PrimaryFormat.Value;
            if (dto.Genre != null) project.Genre = dto.Genre;
            if (dto.Theme != null) project.Theme = dto.Theme;
            if (dto.Tone.HasValue) project.Tone = dto.Tone.Value;
            if (dto.Audience.HasValue) project.Audience = dto.Audience.Value;

            if (dto.ContentMetaInfo != null)
            {
                var success = await SyncIdentityAsync<ProjectIdentityStrategy>(projectId, dto.ContentMetaInfo);
                if(!success) return ApiResponseDto<ProjectResponseDto>.ServerError("Update failed.");
            }

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            await _core.AuditService.LogDbAsync(contextProjectId, "Updated", "Project", project.Id, "Project and identity updated.");

            return ApiResponseDto<ProjectResponseDto>.Success(MapToResponseDto(project));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger?.LogError(ex, "Error updating project");
            return ApiResponseDto<ProjectResponseDto>.ServerError("Update failed.");
        }
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteAsync(Guid projectId, Guid contextProjectId)
    {
         var authorizationError = await CheckAccessAsync<DeleteResponseDto>(contextProjectId, Permission.CanDelete);
        if (authorizationError != null) 
            return authorizationError;

        var project = await _db.Projects.FindAsync(projectId);
        if (project == null) return ApiResponseDto<DeleteResponseDto>.NotFound("Project not found.");

        project.IsDeleted = true;
        project.DeletedAt = DateTime.UtcNow;
        _db.Set<Project>().Update(project);

        await _core.AuditService.LogDbAsync(contextProjectId, "Deleted", "Project", projectId, "Project deleted.");

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto 
        { 
            EntityId = projectId, 
            ProjectId = contextProjectId 
        });
    }

    private ProjectResponseDto MapToResponseDto(Project p) => new()
    {
        Id = p.Id,
        Title = p.ProjectMetaInfo.Title,
        Slug = p.ProjectMetaInfo.Slug,
        Status = p.ProjectMetaInfo.Status,
        ViewMode = p.ProjectMetaInfo.ViewMode,
        CreatedAt = p.ProjectMetaInfo.CreatedAt,
        Description = p.Description,
        IsActive = p.IsActive,
        PrimaryFormat = p.PrimaryFormat,
        Genre = p.Genre,
        Theme = p.Theme,
        Tone = p.Tone,
        Audience = p.Audience
    };
}