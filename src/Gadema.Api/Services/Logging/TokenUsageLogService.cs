// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Activities;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Activities;

/// <summary>
/// Service for tracking API token usage per project.
/// Monitors how many tokens each project has consumed within a billing period.
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
            TokenType = token.TokenType,
            Token = token.Token,
            IsRevoked = token.IsRevoked,
            ExpirationDate = token.ExpirationDate,
            CreatedAt = token.CreatedAt,
        };

    /// <summary>Creates a list response DTO from collection.</summary>
    private ListResponseDto<ProjectTokenResponseDto> CreateListResponseDto(IEnumerable<ProjectToken> tokens)
        => new() { Items = tokens.Select(CreateResponseDto).ToList(), TotalCount = tokens.Count() };

    // ========================================================================
    // GET - List all project tokens for a project
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<ProjectTokenResponseDto>>> GetTokensAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<ProjectTokenResponseDto>>(projectId);
        if (error != null) return error;

        var tokens = await _db.ProjectTokens
            .Where(t => t.ProjectId == projectId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(CreateResponseDto)
            .ToListAsync();

        return ApiResponseDto<ListResponseDto<ProjectTokenResponseDto>>.Success(CreateListResponseDto(tokens));
    }

    // ========================================================================
    // POST - Create a new API token for the current project
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateTokenAsync(Guid projectId, ProjectTokenCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        await _db.SaveChangesAsync();

        // Generate a random API token
        var tokenValue = $"gadema-{Guid.NewGuid():N}-{projectId:N}";

        var token = new ProjectToken
        {
            Id = Guid.NewGuid(),
            TokenType = (ProjectTokenType)createDto.TokenType,
            Token = tokenValue,
            ExpirationDate = createDto.ExpirationDate ?? DateTime.UtcNow.AddYears(1),
            IsRevoked = false,
        };

        _db.ProjectTokens.Add(token);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = token.Id,
            ProjectId = projectId,
            Token = createDto.RevealToken ? tokenValue : null // Only reveal if requested and allowed
        });
    }

    // ========================================================================
    // PUT - Revoke an API token
    // ========================================================================

    public async Task<ApiResponseDto<ProjectTokenResponseDto>> RevokeTokenAsync(Guid id)
    {
        var token = await _db.ProjectTokens.FirstOrDefaultAsync(t => t.Id == id);

        if (token is null)
            return ApiResponseDto<ProjectTokenResponseDto>.NotFound($"API Token with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<ProjectTokenResponseDto>(token.ProjectId);
        if (error != null) return error;

        token.IsRevoked = true;
        token.ExpirationDate = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponseDto<ProjectTokenResponseDto>.Success(CreateResponseDto(token));
    }

    // ========================================================================
    // DELETE - Permanently delete a project token (admin only)
    // ========================================================================

    public async Task<ApiResponseDto<string>> DeleteTokenAsync(Guid id)
    {
        var token = await _db.ProjectTokens.FirstOrDefaultAsync(t => t.Id == id);

        if (token is null)
            return ApiResponseDto<string>.NotFound($"API Token with ID {id} not found.");

        // Only admins can permanently delete tokens
        var user = _userContext.CurrentUser;
        if (user is null || !user.IsAdmin)
            return ApiResponseDto<string>.Forbidden("Only administrators can permanently delete API tokens.");

        _db.ProjectTokens.Remove(token);
        await _db.SaveChangesAsync();

        return ApiResponseDto<string>.Success("API token has been permanently deleted.");
    }

    // ========================================================================
    // GET - Get usage statistics for a project (token consumption tracking)
    // ========================================================================

    public async Task<ApiResponseDto<TokenUsageStats>> GetUsageStatsAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<TokenUsageStats>(projectId);
        if (error != null) return error;

        var totalTokens = await _db.ProjectTokens.CountAsync(t => t.ProjectId == projectId && !t.IsRevoked);
        var revokedTokens = await _db.ProjectTokens.CountAsync(t => t.ProjectId == projectId && t.IsRevoked);
        var activeTokens = totalTokens - revokedTokens;

        return ApiResponseDto<TokenUsageStats>.Success(new TokenUsageStats
        {
            ProjectId = projectId,
            TotalTokens = totalTokens,
            ActiveTokens = activeTokens,
            RevokedTokens = revokedTokens,
            LastUsedAt = await GetLastTokenUsageAsync(projectId)
        });
    }

    // ========================================================================
    // HELPER: Get last token usage timestamp (would track in a separate audit table)
    // ========================================================================

    private async Task<DateTime> GetLastTokenUsageAsync(Guid projectId)
    {
        // In production, this would query a TokenUsageLog table that records
        // each API call made with a project token.
        return DateTime.MinValue;
    }

        // ========================================================================
    // GET - List token usage logs for a project (paginated)
    // ========================================================================

    public async Task<ApiResponseDto<PagedResponseDto<ActivityLogResponseDto>>> GetLogsAsync(
        Guid projectId, 
        int page = 1, 
        int pageSize = 20)
    {
        var error = await ValidateProjectAccessAsync<PagedResponseDto<ActivityLogResponseDto>>(projectId);
        if (error != null) return error;

        // Assuming usage logs are stored in ActivityLogs with EntityType = "TokenUsage"
        var query = _db.ActivityLogs
            .Where(l => l.ProjectId == projectId && l.EntityType == "tokenusage")
            .OrderByDescending(l => l.CreatedAt);

        var total = await query.CountAsync();
        var logs = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return ApiResponseDto<PagedResponseDto<ActivityLogResponseDto>>.Success(new PagedResponseDto<ActivityLogResponseDto>
        {
            Items = logs.Select(CreateActivityLogResponseDto).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }

    // Helper to map ActivityLog to ActivityLogResponseDto (if not already present in ActivityLogService)
    private ActivityLogResponseDto CreateActivityLogResponseDto(ActivityLog log)
        => new()
        {
            Id = log.Id,
            UserId = log.UserId,
            EventType = log.EventType,
            RelatedEntityId = log.RelatedEntityId,
            RelatedEntityType = log.RelatedEntityType,
            Title = log.Title,
            Description = log.Description,
            CreatedAt = log.CreatedAt,
            ProjectId = log.ProjectId
        };


}