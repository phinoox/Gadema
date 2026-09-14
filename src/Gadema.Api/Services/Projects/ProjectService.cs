using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Projects;
using Gadema.Core.Dtos.Search;
using Gadema.Core.Enums;
using Gadema.Core.Models.Projects;
using Gadema.Core.Services;
using Gadema.Core.Interfaces.Identity;

using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;
using Gadema.Api.Services.Search;

namespace Gadema.Api.Services.Projects;

/// <summary>
/// Manages Project domain logic and acts as a searchable provider for the SearchOrchestrator.
/// </summary>
public class ProjectService : CoreService, ISearchableProvider
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

    public async Task<ApiResponseDto<CreateResponseDto>> CreateAsync(Guid projectId, ProjectCreateDto dto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                UserId = _userContext.CurrentUser!.Id,
                Description = dto.Description,
                IsActive = true,
                EnableUserRegistration = dto.EnableUserRegistration,
                AllowManualInvites = dto.AllowManualInvites,
                PrimaryFormat = dto.PrimaryFormat,
                Genre = dto.Genre,
                Theme = dto.Theme,
                Tone = dto.Tone,
                Audience = dto.Audience
            };

            // The identity strategy handles the creation of ContentMetaInfo and its tags
            await _identityStrategy.SyncAsync(project.Id, dto.ContentMetaInfo);

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            await LogDbAsync(projectId, "Created", "Project", project.Id, $"Project '{project.Title}' created.");

            return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto 
            { 
                EntityId = project.Id, 
                ProjectId = projectId 
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
            .Include(p => p.ContentMetaInfo)
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
                .Include(p => p.ContentMetaInfo)
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
        Title = p.ContentMetaInfo.Title,
        Slug = p.ContentMetaInfo.Slug,
        Status = p.ContentMetaInfo.Status,
        Visibility = p.ContentMetaInfo.Visibility,
        ViewMode = p.ContentMetaInfo.ViewMode,
        CreatedAt = p.ContentMetaInfo.CreatedAt,
        Description = p.Description,
        IsActive = p.IsActive,
        PrimaryFormat = p.PrimaryFormat,
        Genre = p.Genre,
        Theme = p.Theme,
        Tone = p.Tone,
        Audience = p.Audience
    };
}