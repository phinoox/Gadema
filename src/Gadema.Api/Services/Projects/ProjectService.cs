using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Projects;
using Gadema.Core.Enums;
using Gadema.Core.Interfaces.Identity;
using Gadema.Core.Models;
using Gadema.Core.Models.Projects;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Projects;

public class ProjectService : CoreService
{
   private readonly IIdentitySyncStrategy _identityStrategy; // Injected strategy

    public ProjectService(
        GameDbContext db, 
        ILogger<ProjectService> logger, 
        IUserContext userContext,
        IIdentitySyncStrategy projectStrategy) // We can inject the specific strategy needed
        : base(db, logger, userContext) 
    {
        // In a more advanced setup, we'd use a factory or named DI to get the right strategy.
        // For now, we assume ProjectService gets its dedicated ProjectIdentityStrategy.
        _identityStrategy = projectStrategy;
    }
    /// <summary>
    /// Creates a new project and its associated MetaInfo.
    /// </summary>
    public async Task<ApiResponseDto<CreateResponseDto>> CreateAsync(Guid projectId, ProjectCreateDto dto)
    {
        // 1. Validation: Ensure user can create projects in this context (if applicable)
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            // 2. Create the Project Anchor
            var project = new Project
            {
                Id = Guid.NewGuid(), // We generate a new ID for the root anchor
                UserId = _userContext.UserId.Value,
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

            // 3. Create the MetaInfo (Identity)
            var metaInfo = new ProjectMetaInfo
            {
                Id = project.Id, // Identity is tied to the Anchor ID
                Title = dto.MetaInfo.Title,
                Slug = dto.MetaInfo.Slug,
                Status = dto.MetaInfo.Status,
                Visibility = dto.MetaInfo.Visibility,
                ViewMode = dto.MetaInfo.ViewMode,
                ProjectId = project.Id,
                CreatedAt = DateTime.Now,
            };

            _db.Projects.Add(project);
            _db.Set<ProjectMetaInfo>().Add(metaInfo); // Adding to the set directly

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            await LogDbAsync(projectId, "Created", "Project", project.Id, $"Project '{metaInfo.Title}' created.");

            return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
            {
                EntityId = project.Id,
                ProjectId = projectId // The context project (if any)
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating project");
            return ApiResponseDto<CreateResponseDto>.ServerError("An error occurred while creating the project.");
        }
    }

    /// <summary>
    /// Retrieves a project and its denormalized response DTO.
    /// </summary>
    public async Task<ApiResponseDto<ProjectResponseDto>> GetAsync(Guid projectId, Guid contextProjectId)
    {
        var error = await ValidateProjectAccessAsync<ProjectResponseDto>(contextProjectId);
        if (error != null) return error;

        var project = await _db.Projects
            .Include(p => p.MetaInfo)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == _userContext.UserId);

        if (project == null) return ApiResponseDto<ProjectResponseDto>.NotFound("Project not found.");

        return ApiResponseDto<ProjectResponseDto>.Success(MapToResponseDto(project));
    }

    /// <summary>
    /// Updates the project domain data and/or its MetaInfo.
    /// </summary>
     public async Task<ApiResponseDto<ProjectResponseDto>> UpdateAsync(Guid projectId, Guid contextProjectId, ProjectUpdateDto dto)
    {
        var error = await ValidateProjectAccessAsync<ProjectResponseDto>(contextProjectId);
        if (error != null) return error;

        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var project = await _db.Projects
                .Include(p => p.MetaInfo)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == _userContext.UserId);

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

            // 2. Delegate Identity Sync to the Strategy
            if (dto.MetaInfo != null)
            {
                await ApplyIdentitySyncAsync(projectId, dto.MetaInfo, _identityStrategy);
            }

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            await LogDbAsync(contextProjectId, "Updated", "Project", project.Id, "Project and identity updated.");

            return ApiResponseDto<ProjectResponseDto>.Success(MapToResponseDto(project));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating project");
            return ApiResponseDto<ProjectResponseDto>.ServerError("An error occurred during the update.");
        }
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteAsync(Guid projectId, Guid contextProjectId)
    {
        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(contextProjectId);
        if (error != null) return error;

        var project = await _db.Projects.FindAsync(projectId);
        if (project == null) return ApiResponseDto<DeleteResponseDto>.NotFound("Project not found.");

        _db.Projects.Remove(project); // Cascade will handle MetaInfo and Members/Tasks
        await _db.SaveChangesAsync();

        await LogDbAsync(contextProjectId, "Deleted", "Project", projectId, "Project deleted.");

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto { EntityId = projectId, ProjectId = contextProjectId });
    }

    private ProjectResponseDto MapToResponseDto(Project p) => new()
    {
        Id = p.Id,
        Title = p.MetaInfo.Title,
        Slug = p.MetaInfo.Slug,
        Status = p.MetaInfo.Status,
        Visibility = p.MetaInfo.Visibility,
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