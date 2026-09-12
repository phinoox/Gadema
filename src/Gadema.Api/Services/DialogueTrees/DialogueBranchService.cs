// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.DialogueTrees;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.DialogueTrees;

/// <summary>
/// Service for managing DialogueBranches within the dialogue trees domain.
/// Handles CRUD operations including MetaInfo creation and authorization.
/// </summary>
public class DialogueBranchService : CoreService
{
    public DialogueBranchService(GameDbContext db, ILogger<DialogueBranchService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List all dialogue branches for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<DialogueBranchResponseDto>>> GetDialogueBranchesAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<DialogueBranchResponseDto>>(projectId);
        if (error != null) return error;

        var branches = await _db.DialogueBranches
            .Include(db => db.MetaInfo)
            .Where(db => db.MetaInfo.ProjectId == projectId)
            .OrderBy(db => db.OrderIndex)
            .Select(db => new DialogueBranchResponseDto
            {
                Id = db.Id,
                MetaInfoId = db.MetaInfoId.Value,
                MetaInfoTitle = db.MetaInfo.Title,
                Status = db.MetaInfo.Status,
                IsPublic = db.MetaInfo.IsPublic,
                CreatedAt = db.MetaInfo.CreatedAt,
                LastModifiedAt = db.MetaInfo.LastModifiedAt,
                Title = db.Title,
                Slug = db.Slug,
                VisualNodeImageUri = db.VisualNodeImageUri,
                CharacterIconUri = db.CharacterIconUri,
                IsRoot = db.IsRoot,
                OrderIndex = db.OrderIndex
            })
            .ToListAsync();

        return ApiResponseDto<IEnumerable<DialogueBranchResponseDto>>.Success(branches);
    }

    // ========================================================================
    // GET - Single dialogue branch by ID
    // ========================================================================

    public async Task<ApiResponseDto<DialogueBranchResponseDto>> GetDialogueBranchAsync(Guid id)
    {
        var branch = await _db.DialogueBranches
            .Include(db => db.MetaInfo)
            .FirstOrDefaultAsync(db => db.Id == id);

        if (branch is null)
            return ApiResponseDto<DialogueBranchResponseDto>.NotFound($"Dialogue branch with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DialogueBranchResponseDto>(branch.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<DialogueBranchResponseDto>.Success(new DialogueBranchResponseDto
        {
            Id = branch.Id,
            MetaInfoId = branch.MetaInfoId.Value,
            MetaInfoTitle = branch.MetaInfo.Title,
            Status = branch.MetaInfo.Status,
            IsPublic = branch.MetaInfo.IsPublic,
            CreatedAt = branch.MetaInfo.CreatedAt,
            LastModifiedAt = branch.MetaInfo.LastModifiedAt,
            Title = branch.Title,
            Slug = branch.Slug,
            VisualNodeImageUri = branch.VisualNodeImageUri,
            CharacterIconUri = branch.CharacterIconUri,
            IsRoot = branch.IsRoot,
            OrderIndex = branch.OrderIndex
        });
    }

    // ========================================================================
    // POST - Create a new dialogue branch
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateDialogueBranchAsync(Guid projectId, DialogueBranchCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var metaInfo = CreateMetaInfo(projectId, ContentTypeEnum.StoryOutline, createDto.CreateData);

        _db.MetaInfos.Add(metaInfo);
        await _db.SaveChangesAsync();

        var branch = new DialogueBranch
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id,
            Title = createDto.Title,
            Slug = createDto.Slug,
            VisualNodeImageUri = createDto.VisualNodeImageUri,
            CharacterIconUri = createDto.CharacterIconUri,
            IsRoot = createDto.IsRoot,
            OrderIndex = createDto.OrderIndex ?? 0,
        };

        _db.DialogueBranches.Add(branch);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = branch.Id,
            MetaInfoId = metaInfo.Id,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a dialogue branch
    // ========================================================================

    public async Task<ApiResponseDto<DialogueBranchResponseDto>> UpdateDialogueBranchAsync(Guid id, DialogueBranchUpdateDto updateDto)
    {
        var branch = await _db.DialogueBranches
            .Include(db => db.MetaInfo)
            .FirstOrDefaultAsync(db => db.Id == id);

        if (branch is null)
            return ApiResponseDto<DialogueBranchResponseDto>.NotFound($"Dialogue branch with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DialogueBranchResponseDto>(branch.MetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(branch.MetaInfo, updateDto.MetaInfo);

        if (updateDto.Title != null)
            branch.Title = updateDto.Title;

        if (updateDto.Slug != null)
            branch.Slug = updateDto.Slug;

        if (updateDto.VisualNodeImageUri != null)
            branch.VisualNodeImageUri = updateDto.VisualNodeImageUri;

        if (updateDto.CharacterIconUri != null)
            branch.CharacterIconUri = updateDto.CharacterIconUri;

        if (updateDto.IsRoot.HasValue)
            branch.IsRoot = updateDto.IsRoot.Value;

        if (updateDto.OrderIndex.HasValue)
            branch.OrderIndex = updateDto.OrderIndex.Value;

        await _db.SaveChangesAsync();

        return ApiResponseDto<DialogueBranchResponseDto>.Success(new DialogueBranchResponseDto
        {
            Id = branch.Id,
            MetaInfoId = branch.MetaInfoId.Value,
            MetaInfoTitle = branch.MetaInfo.Title,
            Status = branch.MetaInfo.Status,
            IsPublic = branch.MetaInfo.IsPublic,
            CreatedAt = branch.MetaInfo.CreatedAt,
            LastModifiedAt = branch.MetaInfo.LastModifiedAt,
            Title = branch.Title,
            Slug = branch.Slug,
            VisualNodeImageUri = branch.VisualNodeImageUri,
            CharacterIconUri = branch.CharacterIconUri,
            IsRoot = branch.IsRoot,
            OrderIndex = branch.OrderIndex
        });
    }

    // ========================================================================
    // DELETE - Remove a dialogue branch
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteDialogueBranchAsync(Guid id)
    {
        var branch = await _db.DialogueBranches
            .Include(db => db.MetaInfo)
            .FirstOrDefaultAsync(db => db.Id == id);

        if (branch is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Dialogue branch with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(branch.MetaInfo.ProjectId);
        if (error != null) return error;

        _db.MetaInfos.Remove(branch.MetaInfo);
        _db.DialogueBranches.Remove(branch);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = branch.MetaInfo.ProjectId
        });
    }
}
