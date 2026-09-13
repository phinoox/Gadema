// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Dtos.Projects;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Projects;

/// <summary>
/// Service for managing Project API Tokens (authentication tokens for external integrations).
/// Handles token generation, revocation, and usage tracking.
/// </summary>
public class TokenUsageLogService : CoreService
{
    public TokenUsageLogService(GameDbContext db, ILogger<TokenUsageLogService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    /// <summary>Creates a response DTO from a ProjectToken entity.</summary>
    private ProjectTokenResponseDto CreateResponseDto(ProjectToken token)
        => new()
        {
            Id = token.Id,
            MetaInfoId = token.MetaInfoId,
            TokenType = (int)token.TokenType,
            Token = token.IsRevoked ? null : token.Token,
            IsRevoked = token.IsRevoked,
            ExpirationDate = token.ExpirationDate,
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
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project is null) return ApiResponseDto<ProjectTokenResponseDto>.NotFound($"Project with ID {projectId} not found.");

        // Generate a random API token string
        var tokenValue = $"gadema_{projectId}_{Guid.NewGuid():N}";

        // Determine expiration date based on request or default to 1 year from now
        var expiresAt = createDto.ExpirationDate ?? DateTime.UtcNow.AddYears(1);

        var token = new ProjectToken
        {
            Id = Guid.NewGuid(),
            MetaInfoId = project.Id, // FK: MetaInfo has PK = Project.Id for this relationship
            TokenType = (ProjectTokenType)createDto.TokenType ?? ProjectTokenType.ReadWrite,
            Token = tokenValue,
            ExpirationDate = expiresAt,
            IsRevoked = false,
        };

        _db.ProjectTokens.Add(token);
        await _db.SaveChangesAsync();

        return ApiResponseDto<ProjectTokenResponseDto>.Success(new ProjectTokenResponseDto
        {
            Id = token.Id,
            TokenType = (int)token.TokenType,
            Token = createDto.RevealToken ? tokenValue : null,
            IsRevoked = false,
            ExpirationDate = expiresAt,
            CreatedAt = DateTime.UtcNow,
        });
    }

    // ========================================================================
    // GET /api/v1/projects/{projectId}/tokens — List all tokens for a project
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<ProjectTokenResponseDto>>> GetTokensAsync(Guid projectId)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project is null) return ApiResponseDto<ListResponseDto<ProjectTokenResponseDto>>.NotFound($"Project with ID {projectId} not found.");

        var tokens = await _db.ProjectTokens
            .Include(t => t.MetaInfo)
            .Where(t => t.MetaInfo.ProjectId == projectId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(CreateResponseDto)
            .ToListAsync();

        return ApiResponseDto<ListResponseDto<ProjectTokenResponseDto>>.Success(new ListResponseDto<ProjectTokenResponseDto> { Items = tokens, TotalCount = tokens.Count });
    }

    // ========================================================================
    // DELETE /api/v1/projects/{projectId}/tokens/{tokenId} — Revoke a token
    // ========================================================================

    public async Task<ApiResponseDto<string>> RevokeTokenAsync(Guid tokenId)
    {
        var token = await _db.ProjectTokens.Include(t => t.MetaInfo).FirstOrDefaultAsync(t => t.Id == tokenId);

        if (token is null) return ApiResponseDto<string>.NotFound($"API Token with ID {tokenId} not found.");

        // Check project access
        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(token.MetaInfo.ProjectId);
        if (error != null) return error;

        token.IsRevoked = true;
        token.ExpirationDate = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return ApiResponseDto<string>.Success($"API Token {tokenId} has been revoked.");
    }

    // ========================================================================
    // GET /api/v1/projects/{projectId}/tokens/stats — Get usage statistics
    // ========================================================================

    public async Task<ApiResponseDto<TokenUsageStats>> GetUsageStatsAsync(Guid projectId)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project is null) return ApiResponseDto<TokenUsageStats>.NotFound($"Project with ID {projectId} not found.");

        // In production, this would query a TokenUsageLog table that records each API call.
        // For now, return placeholder stats.
        return ApiResponseDto<TokenUsageStats>.Success(new TokenUsageStats
        {
            ProjectId = projectId,
            TotalTokens = await _db.ProjectTokens.CountAsync(t => t.MetaInfo.ProjectId == projectId),
            ActiveTokens = await _db.ProjectTokens.CountAsync(t => t.MetaInfo.ProjectId == projectId && !t.IsRevoked),
            RevokedTokens = await _db.ProjectTokens.CountAsync(t => t.MetaInfo.ProjectId == projectId && t.IsRevoked),
            LastUsedAt = DateTime.MinValue, // Would be populated from a usage log table
        });
    }
}