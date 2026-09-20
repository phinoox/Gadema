// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Access;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Base.Projects;

/// <summary>
/// Service for managing Project API Tokens (authentication tokens for external integrations).
/// Handles token generation, revocation, and usage tracking.
/// </summary>
[ServiceLifetime(ServiceLifetime.Scoped)] public class ProjectTokenService : CoreService
{
    public ProjectTokenService(GameDbContext db, ILogger<ProjectTokenService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    /// <summary>Creates a response DTO from a ProjectToken entity.</summary>
    private ProjectTokenResponseDto CreateResponseDto(ProjectToken token)
        => new()
        {
            Id = token.Id,
            TokenName = token.TokenName, 
            TokenType = 1, // Defaulting to ReadWrite (1)
            Token = null,  // Never return raw token in list view for security
            IsRevoked = !token.IsActive, 
            ExpirationDate = token.ExpiresAt,
            CreatedAt = token.CreatedAt,
        };

    /// <summary>Creates a list response DTO from collection.</summary>
    private ListResponseDto<ProjectTokenResponseDto> CreateListResponseDto(IEnumerable<ProjectToken> tokens)
        => new() { Items = tokens.Select(CreateResponseDto).ToList(), TotalCount = tokens.Count() };

    // ========================================================================
    // POST /api/v1/projects/{projectId}/tokens — Create a new API token
    // ========================================================================

    public async Task<ApiResponseDto<ProjectTokenResponseDto>> CreateTokenAsync(Guid projectId, ProjectTokenCreateDto createDto)
    {
        var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists) return ApiResponseDto<ProjectTokenResponseDto>.NotFound($"Project with ID {projectId} not found.");

        var rawTokenValue = $"gadema_{projectId}_{Guid.NewGuid():N}";

        var token = new ProjectToken
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            TokenName = createDto.TokenName ?? "New API Token", 
            TokenHash = rawTokenValue, // In production: Hash this!
            MaxRequests = createDto.MaxRequests, // Using the new property
            CurrentUsage = 0,                  // Starts at zero
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

    // ========================================================================
    // GET /api/v1/projects/{projectId}/tokens — List all tokens for a project
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<ProjectTokenResponseDto>>> GetTokensAsync(Guid projectId)
    {
        var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists) return ApiResponseDto<ListResponseDto<ProjectTokenResponseDto>>.NotFound($"Project not found.");

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

        var user = _userContext.CurrentUser;
        if (user is null )
            return ApiResponseDto<string>.Forbidden("Only administrators can permanently delete API tokens.");
        
        if (!await IsAdminAsync(token.ProjectId))
            return ApiResponseDto<string>.Forbidden("Only administrators can permanently delete API tokens.");

        // Capture info before deletion to log it
        var projectId = token.ProjectId;
        var tokenName = token.TokenName;

        _db.ProjectTokens.Remove(token);
        await _db.SaveChangesAsync();

        // --- NEW: LOG THE DELETION ---
        await LogDbAsync(projectId, "Deleted", nameof(ProjectToken), id, $"Permanently deleted API token: {tokenName}");

        return ApiResponseDto<string>.Success($"API token has been permanently deleted.");
    }

    // ========================================================================
    // DELETE /api/v1/projects/{projectId}/tokens/{tokenId} — Revoke a token
    // ========================================================================

    public async Task<ApiResponseDto<string>> RevokeTokenAsync(Guid tokenId)
    {
        var token = await _db.ProjectTokens.FindAsync(tokenId);
        if (token is null) return ApiResponseDto<string>.NotFound($"API Token with ID {tokenId} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(token.ProjectId);
        if (error != null) return ApiResponseDto<string>.Unauthorized(error.Message);

        token.IsActive = false;
        await _db.SaveChangesAsync();

        await LogDbAsync(token.ProjectId, "Revoked", nameof(ProjectToken), token.Id, $"Revoked API token: {token.TokenName}");

        return ApiResponseDto<string>.Success($"API Token {tokenId} has been revoked.");
    }

    // ========================================================================
    // GET /api/v1/projects/{projectId}/tokens/stats — Get usage statistics
    // ========================================================================

    public async Task<ApiResponseDto<TokenUsageStats>> GetUsageStatsAsync(Guid projectId)
    {
        var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists) return ApiResponseDto<TokenUsageStats>.NotFound($"Project not found.");

        // Find the most recent active token to report its usage/quota status
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

