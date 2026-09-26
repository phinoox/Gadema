using Gadema.Api.CoreServices;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Access;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Data.Database.Core;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Base.Projects;

[ServiceLifetime(ServiceLifetime.Scoped)] 
public class ProjectTokenService : DomainService
{
    private CoreDbContext _db;

    public ProjectTokenService(CoreDbContext db,
        ILogger<ProjectTokenService> logger,  
        CoreServicesProvider coreServices) // Injected via CoreService constructor
        : base(coreServices,logger)
    {
         _db = db;
    }

    private ProjectTokenResponseDto CreateResponseDto(ProjectToken token)
        => new()
        {
            Id = token.Id,
            TokenName = token.TokenName, 
            TokenType = 1, 
            Token = null,  
            IsRevoked = !token.IsActive, 
            ExpirationDate = token.ExpiresAt,
            CreatedAt = token.CreatedAt,
        };

    private ListResponseDto<ProjectTokenResponseDto> CreateListResponseDto(IEnumerable<ProjectToken> tokens)
        => new() { Items = tokens.Select(CreateResponseDto).ToList(), TotalCount = tokens.Count() };

    public async Task<ApiResponseDto<ProjectTokenResponseDto>> CreateTokenAsync(Guid projectId, ProjectTokenCreateDto createDto)
    {
        // Check if user has permission to edit the project before creating a token
        var error = await CheckAccessAsync<ProjectTokenResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists) return ApiResponseDto<ProjectTokenResponseDto>.NotFound($"Project with ID {projectId} not found.");

        var rawTokenValue = $"gadema_{projectId}_{Guid.NewGuid():N}";

        var token = new ProjectToken
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            TokenName = createDto.TokenName ?? "New API Token", 
            TokenHash = rawTokenValue, 
            MaxRequests = createDto.MaxRequests, 
            CurrentUsage = 0,                  
            IsActive = true,
            ExpiresAt = createDto.ExpirationDate,
            CreatedAt = DateTime.UtcNow
        };

        _db.ProjectTokens.Add(token);
        await _db.SaveChangesAsync();

         await LogDbAsync(projectId, "Created", nameof(ProjectToken), token.Id, $"Created new API token: {token.TokenName}");

        return ApiResponseDto<ProjectTokenResponseDto>.Success(new ProjectTokenResponseDto
        {
            Id = token.Id,
            TokenName = token.TokenName,
            TokenType = 1,
            Token = createDto.RevealToken ? rawTokenValue : null,
            IsRevoked = false,
            ExpirationDate = token.ExpiresAt,
            CreatedAt = token.CreatedAt
        });
    }

    public async Task<ApiResponseDto<ListResponseDto<ProjectTokenResponseDto>>> GetTokensAsync(Guid projectId)
    {
        // Check if user has permission to view the project
        var error = await CheckAccessAsync<ListResponseDto<ProjectTokenResponseDto>>(projectId, Permission.CanView);
        if (error != null) return error;

        var tokens = await _db.ProjectTokens
            .Where(t => t.ProjectId == projectId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return ApiResponseDto<ListResponseDto<ProjectTokenResponseDto>>.Success(CreateListResponseDto(tokens));
    }

    public async Task<ApiResponseDto<string>> DeleteTokenAsync(Guid id)
    {
        var token = await _db.ProjectTokens.FirstOrDefaultAsync(t => t.Id == id);
        if (token is null) return ApiResponseDto<string>.NotFound($"API Token with ID {id} not found.");

        // Check if user has permission to edit the project that owns this token
        var error = await CheckAccessAsync<string>(token.ProjectId, Permission.CanEdit);
        if (error != null) return error;

        // Additionally check for Admin role as per original logic requirement for permanent deletion
        if (!await IsAdminAsync(token.ProjectId))
            return ApiResponseDto<string>.Forbidden("Only administrators can permanently delete API tokens.");

        var projectId = token.ProjectId;
        var tokenName = token.TokenName;

        _db.ProjectTokens.Remove(token);
        await _db.SaveChangesAsync();

        await LogDbAsync(projectId, "Deleted", nameof(ProjectToken), id, $"Permanently deleted API token: {tokenName}");

        return ApiResponseDto<string>.Success($"API token has been permanently deleted.");
    }

    public async Task<ApiResponseDto<string>> RevokeTokenAsync(Guid tokenId)
    {
        var token = await _db.ProjectTokens.FindAsync(tokenId);
        if (token is null) return ApiResponseDto<string>.NotFound($"API Token with ID {tokenId} not found.");

        // Check if user has permission to edit the project
        var error = await CheckAccessAsync<string>(token.ProjectId, Permission.CanEdit);
        if (error != null) return error;

        token.IsActive = false;
        await _db.SaveChangesAsync();

        await LogDbAsync(token.ProjectId, "Revoked", nameof(ProjectToken), token.Id, $"Revoked API token: {token.TokenName}");

        return ApiResponseDto<string>.Success($"API Token {tokenId} has been revoked.");
    }

    
    public async Task<ApiResponseDto<TokenUsageStats>> GetUsageStatsAsync(Guid projectId)
    {
        // Check if user has permission to view the project
        var error = await CheckAccessAsync<TokenUsageStats>(projectId, Permission.CanView);
        if (error != null) return error;

        var token = await _db.ProjectTokens
            .Where(t => t.ProjectId == projectId && t.IsActive)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync();

        if (token is null)
        {
             return ApiResponseDto<TokenUsageStats>.Success(new TokenUsageStats
             {
                 TotalUsage = 0,
                 RemainingUsage = 0,
                 ExpiresAt = null,
                 IsExpired = false
             });
        }

        var now = DateTime.UtcNow;
        bool isExpired = token.ExpiresAt.HasValue && token.ExpiresAt <= now;

        return ApiResponseDto<TokenUsageStats>.Success(new TokenUsageStats
        {
            TotalUsage = token.CurrentUsage,
            RemainingUsage = token.MaxRequests > 0 ? Math.Max(0, token.MaxRequests - token.CurrentUsage) : 0,
            ExpiresAt = token.ExpiresAt,
            IsExpired = isExpired
        });
    }
}