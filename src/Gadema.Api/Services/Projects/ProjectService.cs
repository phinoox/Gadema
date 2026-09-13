// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Dtos.Projects;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Models.Projects;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Projects;

/// <summary>
/// Service for managing Projects - the foundational container for all content.
/// Handles project CRUD, team management, and API token lifecycle.
/// </summary>
public class ProjectService : CoreService
{
    public ProjectService(GameDbContext db, ILogger<ProjectService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    /// <summary>Creates a response DTO from a Project entity.</summary>
    private ProjectResponseDto CreateResponseDto(Project project)
        => new()
        {
            Id = project.Id,
            Title = project.Title,
            Slug = project.Slug,
            Description = project.Description,
            Status = project.Status,
            Visibility = project.Visibility,
            CreatedAt = project.CreatedAt,
            LastModifiedAt = project.LastModifiedAt,
        };

    /// <summary>Creates a list response DTO from collection.</summary>
    private ListResponseDto<ProjectResponseDto> CreateListResponseDto(IEnumerable<Project> projects)
        => new() { Items = projects.Select(CreateResponseDto).ToList(), TotalCount = projects.Count() };

    // ========================================================================
    // GET /api/v1/projects — List all user's projects (paginated, filterable)
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<ProjectResponseDto>>> GetProjectsAsync(
        int page = 1,
        int pageSize = 20,
        string? status = null,
        ProjectVisibilityEnum? visibility = null,
        string? searchQuery = null)
    {
        var user = _userContext.CurrentUser;
        if (user == null) return ApiResponseDto<ListResponseDto<ProjectResponseDto>>.Unauthorized("Not authenticated.");

        IQueryable<Project> query = _db.Projects.Where(p => p.UserId == user.Id).OrderByDescending(p => p.CreatedAt);

        // Filter by status
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(p => Enum.TryParse(status, ignoreCase: true, out ProjectStatusEnum s) && p.Status == s);

        // Filter by visibility (0=Private, 1=Team, 2=Public)
        if (visibility.HasValue)
            query = query.Where(p => p.Visibility >= visibility.Value);

        // Search filter (searches title and description)
        if (!string.IsNullOrWhiteSpace(searchQuery))
            query = query.Where(p => p.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                                    p.Description?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) == true);

        var total = await query.CountAsync();
        var projects = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return ApiResponseDto<ListResponseDto<ProjectResponseDto>>.Success(new ListResponseDto<ProjectResponseDto>
        {
            
            Items = CreateListResponseDto(projects),
            TotalCount = total,
        });
    }

    // ========================================================================
    // GET /api/v1/projects/{id} — Single project by ID
    // ========================================================================

    public async Task<ApiResponseDto<ProjectResponseDto>> GetProjectAsync(Guid id)
    {
        var project = await _db.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == id);

        if (project is null)
            return ApiResponseDto<ProjectResponseDto>.NotFound($"Project with ID {id} not found.");

        // Authorization: only the owner can access their own projects via direct GET
        var user = _userContext.CurrentUser;
        if (user == null || project.UserId != user.Id)
            return ApiResponseDto<ProjectResponseDto>.Forbidden("You do not have access to this project.");

        return ApiResponseDto<ProjectResponseDto>.Success(CreateResponseDto(project));
    }

    // ========================================================================
    // POST /api/v1/projects — Create a new project
    // ========================================================================

    public async Task<ApiResponseDto<ProjectResponseDto>> CreateProjectAsync(ProjectCreateDto createDto)
    {
        var user = _userContext.CurrentUser;
        if (user == null) return ApiResponseDto<ProjectResponseDto>.Unauthorized("Not authenticated.");

        // Generate slug from title
        var slug = string.IsNullOrWhiteSpace(createDto.Slug) ? GenerateSlug(createDto.Title) : createDto.Slug;

        var project = new Project
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Title = createDto.Title,
            Slug = slug,
            Description = createDto.Description,
            Status = createDto.Status,
            Visibility = createDto.Visibility,
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();
     
        return ApiResponseDto<ProjectResponseDto>.Success(CreateResponseDto(project));
    }

    // ========================================================================
    // PUT /api/v1/projects/{id} — Update a project
    // ========================================================================

    public async Task<ApiResponseDto<ProjectResponseDto>> UpdateProjectAsync(Guid id, ProjectUpdateDto updateDto)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == id);

        if (project is null)
            return ApiResponseDto<ProjectResponseDto>.NotFound($"Project with ID {id} not found.");

        // Authorization: only owner can update
        var user = _userContext.CurrentUser;
        if (user == null || project.UserId != user.Id)
            return ApiResponseDto<ProjectResponseDto>.Forbidden("You do not have permission to edit this project.");

        if (!string.IsNullOrWhiteSpace(updateDto.Title)) project.Title = updateDto.Title;
        if (updateDto.Description != null) project.Description = updateDto.Description;
        if (updateDto.Status.HasValue) project.Status = (ProjectStatusEnum)updateDto.Status.Value;
        if (updateDto.Visibility.HasValue) project.Visibility = updateDto.Visibility.Value;

        project.LastModifiedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return ApiResponseDto<ProjectResponseDto>.Success(CreateResponseDto(project));
    }

    // ========================================================================
    // DELETE /api/v1/projects/{id} — Transfer or delete a project
    // ========================================================================

    public async Task<ApiResponseDto<string>> TransferOrDeleteProjectAsync(Guid id)
    {
        var project = await _db.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == id);

        if (project is null)
            return ApiResponseDto<string>.NotFound($"Project with ID {id} not found.");

        // Authorization: only owner can transfer/delete
        var user = _userContext.CurrentUser;
        if (user == null || project.UserId != user.Id)
            return ApiResponseDto<string>.Forbidden("You do not have permission to modify this project.");

        var action = request.Method.Equals(HttpMethods.Delete, StringComparison.OrdinalIgnoreCase) ? "delete" : "transfer";

        // Soft delete: set status to Deleted, don't cascade delete MetaInfos (they might be referenced elsewhere)
        project.Status = ProjectStatusEnum.Deleted;
        await _db.SaveChangesAsync();

        return ApiResponseDto<string>.Success($"Project '{project.Title}' has been {action}d successfully.");
    }

    // ========================================================================
    // POST /api/v1/projects/{id}/tokens — Create API token (via ProjectTokenService)
    // ========================================================================

    public async Task<ApiResponseDto<ProjectTokenResponseDto>> CreateApiTokenAsync(Guid projectId, ProjectTokenCreateDto createDto)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project is null) return ApiResponseDto<ProjectTokenResponseDto>.NotFound($"Project with ID {projectId} not found.");

        // Authorization: only owner can create tokens
        var user = _userContext.CurrentUser;
        if (user == null || project.UserId != user.Id)
            return ApiResponseDto<ProjectTokenResponseDto>.Forbidden("You do not have permission to manage API tokens for this project.");

        var tokenService = new Gadema.Api.Services.Projects.TokenUsageLogService(_db, _logger, _userContext);
        return await tokenService.CreateTokenAsync(projectId, createDto);
    }

    // ========================================================================
    // GET /api/v1/projects/{id}/tokens — List API tokens (via ProjectTokenService)
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<ProjectTokenResponseDto>>> GetProjectTokensAsync(Guid projectId)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project is null) return ApiResponseDto<ListResponseDto<ProjectTokenResponseDto>>.NotFound($"Project with ID {projectId} not found.");

        // Authorization: only owner can list tokens
        var user = _userContext.CurrentUser;
        if (user == null || project.UserId != user.Id)
            return ApiResponseDto<ListResponseDto<ProjectTokenResponseDto>>.Forbidden("You do not have permission to access API tokens for this project.");

        var tokenService = new Gadema.Api.Services.Projects.TokenUsageLogService(_db, _logger, _userContext);
        return await tokenService.GetTokensAsync(projectId);
    }

    // ========================================================================
    // DELETE /api/v1/projects/{id}/tokens/{tokenId} — Revoke API token (via ProjectTokenService)
    // ========================================================================

    public async Task<ApiResponseDto<string>> RevokeApiTokenAsync(Guid projectId, Guid tokenId)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project is null) return ApiResponseDto<string>.NotFound($"Project with ID {projectId} not found.");

        // Authorization: only owner can revoke tokens
        var user = _userContext.CurrentUser;
        if (user == null || project.UserId != user.Id)
            return ApiResponseDto<string>.Forbidden("You do not have permission to manage API tokens for this project.");

        var tokenService = new Gadema.Api.Services.Projects.TokenUsageLogService(_db, _logger, _userContext);
        await tokenService.RevokeTokenAsync(tokenId);

        return ApiResponseDto<string>.Success($"API Token {tokenId} has been revoked successfully.");
    }
}