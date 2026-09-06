// =============================================================================
using Gadema.Core.Dtos;
using Microsoft.EntityFrameworkCore;
// Gadema.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Gadema.Core.Dtos.Projects;
using Gadema.Data.Database;
using Microsoft.Extensions.Logging;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Services;
using Gadema.Core.Models.Projects;

namespace Gadema.Api.Services;

/// <summary>
/// Implementation of project service.
/// </summary>
public class ProjectService : IGademaService,  IProjectService
{
    private readonly GameDbContext _context;

    private readonly IUserContext _userContext;
    private readonly ILogger<ProjectService> _logger;

    public ServiceTypeEnum ServiceType => ServiceTypeEnum.ProjectService;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public ProjectService(GameDbContext context, ILogger<ProjectService> logger,IUserContext userContext)
    {
        _context = context;
        _logger = logger;
        _userContext = userContext;
    }

    /// <summary>
    /// List all projects (paginated).
    /// </summary>
    public async Task<ApiResponseDto<PaginationResponse<ProjectResponseDto>>> GetProjectsAsync(
        int page,
        int pageSize,
        string? status = null,
        int? visibility = null,
        string? search = null)
    {
        // Implement project listing logic
        var projects = await _context.Projects.ToListAsync();
        return ApiResponseDto<PaginationResponse<ProjectResponseDto>>.Success(new PaginationResponse<ProjectResponseDto>());
    }

    /// <summary>
    /// Create a new project.
    /// </summary>
    public async Task<ApiResponseDto<ProjectResponseDto>> CreateProjectAsync(CreateProjectDto createDto)
    {
        var user = _userContext.CurrentUser;

        if (user == null)
            return ApiResponseDto<ProjectResponseDto>.Unauthorized("Not authenticated.");

        var project = new Project();
        project.User = user;
        project.Id = new Guid();
        project.Description = createDto.Description;
        project.Title = createDto.Title;
        project.Visibility = createDto.Visibility;
        project.Slug = createDto.Slug ?? createDto.Title.Trim();
        _context.Projects.Add(project);
        _context.SaveChanges();
        // Implement project creation logic
        return ApiResponseDto<ProjectResponseDto>.Success(new ProjectResponseDto()
        {
            Id = project.Id,
            Title = project.Title,
            Visibility = project.Visibility,
            Status = project.Status,
            OwnerId = user.Id
        }
        );
    }

    /// <summary>
    /// Update a project.
    /// </summary>
    public async Task<ApiResponseDto<ProjectResponseDto>> UpdateProjectAsync(Guid id, UpdateProjectDto updateDto)
    {
        // Implement project update logic
        return ApiResponseDto<ProjectResponseDto>.Success(new ProjectResponseDto());
    }

    /// <summary>
    /// Transfer or delete a project.
    /// </summary>
    public async Task<ApiResponseDto<SimpleResponseDto>> TransferOrDeleteProjectAsync(Guid id)
    {
        // Implement transfer/delete logic
        return ApiResponseDto<SimpleResponseDto>.Success(new SimpleResponseDto(){ Success = true, Message = "Project has been transferred successfully" });
    }

    /// <summary>
    /// Create API token for project.
    /// </summary>
    public async Task<ApiResponseDto<ProjectTokenResponseDto>> CreateApiTokenAsync(Guid id, ProjectTokenDto tokenDto)
    {
        // Implement API token creation logic
        return ApiResponseDto<ProjectTokenResponseDto>.Success(new ProjectTokenResponseDto());
    }

    /// <summary>
    /// List API tokens for project.
    /// </summary>
    public async Task<ApiResponseDto<TokenListResponseDto>> GetProjectTokensAsync(Guid id)
    {
        // Implement API token listing logic
        return ApiResponseDto<TokenListResponseDto>.Success(new TokenListResponseDto());
    }

    /// <summary>
    /// Revoke API token for project.
    /// </summary>
    public async Task<ApiResponseDto<SimpleResponseDto>> RevokeApiTokenAsync(Guid id, Guid tokenId)
    {
        // Implement API token revocation logic
        return ApiResponseDto<SimpleResponseDto>.Success(new SimpleResponseDto(){ Success = true, Message = "Token has been revoked successfully" });
    }

    Task<ApiResponseDto<object>> IProjectService.TransferOrDeleteProjectAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    Task<ApiResponseDto<object>> IProjectService.RevokeApiTokenAsync(Guid id, Guid tokenId)
    {
        throw new NotImplementedException();
    }
}