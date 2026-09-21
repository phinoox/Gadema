using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Search;

using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;
using Gadema.Api.Services.Search;
using Gadema.Core.Interfaces;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Models.Base.Projects;

namespace Gadema.Api.Services.Base.Projects;

/// <summary>
/// Manages Project domain logic and acts as a searchable provider for the SearchOrchestrator.
/// </summary>
[ServiceLifetime(ServiceLifetime.Scoped)] public class ProjectService : CoreService, ISearchableProvider
{
    private readonly IIdentitySyncStrategy _identityStrategy;

    public ProjectService(
        GameDbContext db, 
        ILogger<ProjectService> logger, 
        IUserContext userContext,
        IIdentitySyncStrategy projectIdentityStrategy) 
        : base(db, logger, userContext)
    {
        _identityStrategy = projectIdentityStrategy;
    }

    // ========================================================================
    // SEARCH PROVIDER IMPLEMENTATION (The "Read" Strategy)
    // ========================================================================

    /// <summary>
    /// Implements ISearchableProvider. Provides matches for the SearchOrchestrator.
    /// </summary>
   public async Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId)
    {
        // We query the MetaInfos table because that is where the searchable 
        // identity data (Title, Slug) actually resides.
        var metaQuery = _db.MetaInfos.AsQueryable();

        // If a scope is provided, filter by ProjectId
        if (projectId.HasValue)
        {
            metaQuery = metaQuery.Where(m => m.ProjectId == projectId.Value);
        }

        // Perform text-based discovery on the Identity properties
        return await metaQuery
            .Where(m => m.Title.Contains(query) || m.Slug.Contains(query))
            .Select(m => new SearchHitDto
            {
                ResourceId = m.Id, // The anchor ID
                DisplayName = m.Title,
                Slug = m.Slug,
                ResourceType = "Project",
                ScopeId = null, // Projects are the root level
                ResourceLink = $"/api/v1/projects/{m.Id}"
            })
            .ToListAsync();
    }

    // ========================================================================
    // DOMAIN OPERATIONS (The "Write" Side)
    // ========================================================================

       public async Task<ApiResponseDto<CreateResponseDto>> CreateAsync(ProjectCreateDto createDto)
    {
        // 1. Validation: Check if user is authenticated and authorized to create projects.
        // We no longer check 'projectId' access because the project doesn't exist yet.
        if (_userContext.CurrentUser == null)
        {
            return ApiResponseDto<CreateResponseDto>.Unauthorized("User must be authenticated.");
        }

        var meta = CreateMetaInfo<ProjectSeriesMetaInfo>(createDto.MetaInfo, m => {
            // Title and Slug are handled by the base class logic inside CreateMetaInfo<T>
        });

        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var project = new Project
            {
                Id = Guid.NewGuid(), // The actual ID of the new project
                MetaInfoId = meta.Id,
                UserId = _userContext.CurrentUser.Id,
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

            // 2. Logging: Log with null context since this is a root-level creation.
            await LogDbAsync(null, "Created", "Project", project.Id, $"Project '{project.MetaInfo.Title}' created.");

            return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto 
            { 
                EntityId = project.Id, 
                ProjectId = project.Id // The new ID is the ProjectId
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
        var error = await ValidateProjectAccessAsync<ProjectResponseDto>(contextProjectId);
        if (error != null) return error;

        var project = await _db.Projects
            .Include(p => p.MetaInfo)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null) return ApiResponseDto<ProjectResponseDto>.NotFound("Project not found.");

        return ApiResponseDto<ProjectResponseDto>.Success(MapToResponseDto(project));
    }

    public async Task<ApiResponseDto<ProjectResponseDto>> UpdateAsync(Guid projectId, Guid contextProjectId, ProjectUpdateDto dto)
    {
        var error = await ValidateProjectAccessAsync<ProjectResponseDto>(contextProjectId);
        if (error != null) return error;

        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var project = await _db.Projects
                .Include(p => p.MetaInfo)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null) return ApiResponseDto<ProjectResponseDto>.NotFound("Project not found.");

            // 1. Update Domain Data
            if (dto.Description != null) project.Description = dto.Description;
            if (dto.EnableUserRegistration.HasValue) project.EnableUserRegistration = dto.EnableUserRegistration.Value;
            if (dto.AllowManualInvites.HasValue) project.AllowManualInvites = dto.AllowManualInvites.Value;
            if (dto.PrimaryFormat.HasValue) project.PrimaryFormat = dto.PrimaryFormat.Value;
            if (dto.Genre != null) project.Genre = dto.Genre;
            if (dto.Theme != null) project.Theme = dto.Theme;
            if (dto.Tone.HasValue) project.Tone = dto.Tone.Value;
            if (dto.Audience.HasValue) project.Audience = dto.Audience.Value;

            // 2. Sync Identity via Strategy (Handles ProjectMetaInfo and Tags)
            if (dto.ContentMetaInfo != null)
            {
                await _identityStrategy.SyncAsync(projectId, dto.ContentMetaInfo);
            }

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            await LogDbAsync(contextProjectId, "Updated", "Project", project.Id, "Project and identity updated.");

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
        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(contextProjectId);
        if (error != null) return error;

        var project = await _db.Projects.FindAsync(projectId);
        if (project == null) return ApiResponseDto<DeleteResponseDto>.NotFound("Project not found.");

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();

        await LogDbAsync(contextProjectId, "Deleted", "Project", projectId, "Project deleted.");

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto 
        { 
            EntityId = projectId, 
            ProjectId = contextProjectId 
        });
    }

    private ProjectResponseDto MapToResponseDto(Project p) => new()
    {
        Id = p.Id,
        Title = p.MetaInfo.Title,
        Slug = p.MetaInfo.Slug,
        Status = p.MetaInfo.Status,
        ViewMode = p.MetaInfo.ViewMode,
        CreatedAt = p.MetaInfo.CreatedAt,
        Description = p.Description,
        IsActive = p.IsActive,
        PrimaryFormat = p.PrimaryFormat,
        Genre = p.Genre,
        Theme = p.Theme,
        Tone = p.Tone,
        Audience = p.Audience
    };
}