// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.DialogueTrees;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Content;

/// <summary>
/// Service for managing DialogueBranches - the tree structure of dialogue nodes.
/// A branch represents a complete dialogue path from root to leaf.
/// </summary>
public class DialogueBranchService : CoreService
{
    public DialogueBranchService(GameDbContext db, ILogger<DialogueBranchService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    private DialogueBranchResponseDto CreateResponseDto(DialogueBranch branch)
        => new()
        {
            Id = branch.Id,
            MetaInfoId = branch.MetaInfoId,
            Title = branch.Title,
            Slug = branch.Slug,
            ShortDesc = branch.ShortDesc,
            Status = (int)branch.Status,
            IsPublic = branch.IsPublic,
            RootNodeId = branch.RootNodeId,
            CreatedAt = branch.CreatedAt,
        };

    public async Task<ApiResponseDto<IEnumerable<DialogueBranchResponseDto>>> GetBranchesAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<DialogueBranchResponseDto>>(projectId);
        if (error != null) return error;

        var branches = await _db.DialogueBranches
            .Include(db => db.MetaInfo)
            .Where(db => db.MetaInfo.ProjectId == projectId)
            .OrderByDescending(db => db.CreatedAt)
            .Select(CreateResponseDto)
            .ToListAsync();

        return ApiResponseDto<IEnumerable<DialogueBranchResponseDto>>.Success(branches);
    }

    public async Task<ApiResponseDto<DialogueBranchResponseDto>> GetBranchAsync(Guid id)
    {
        var branch = await _db.DialogueBranches.Include(db => db.MetaInfo).FirstOrDefaultAsync(db => db.Id == id);
        if (branch is null) return ApiResponseDto<DialogueBranchResponseDto>.NotFound($"Dialogue branch with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DialogueBranchResponseDto>(branch.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<DialogueBranchResponseDto>.Success(CreateResponseDto(branch));
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateBranchAsync(Guid projectId, DialogueBranchCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var metaInfo = CreateMetaInfo(projectId, ContentTypeEnum.DialogueBranch, createDto.CreateData);

        _db.MetaInfos.Add(metaInfo);
        await _db.SaveChangesAsync();

        // Find or create root node
        var rootNode = await GetOrCreateRootNodeAsync(projectId, createDto.RootNodeId);

        var branch = new DialogueBranch
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id.Value,
            Title = createDto.Title,
            Slug = createDto.Slug,
            ShortDesc = createDto.ShortDesc,
            Status = (ContentStatusEnum)createDto.Status,
            IsPublic = createDto.IsPublic,
            RootNodeId = rootNode.Id,
        };

        _db.DialogueBranches.Add(branch);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto { EntityId = branch.Id, MetaInfoId = metaInfo.Id.Value, ProjectId = projectId });
    }

    public async Task<ApiResponseDto<DialogueBranchResponseDto>> UpdateBranchAsync(Guid id, DialogueBranchUpdateDto updateDto)
    {
        var branch = await _db.DialogueBranches.Include(db => db.MetaInfo).FirstOrDefaultAsync(db => db.Id == id);
        if (branch is null) return ApiResponseDto<DialogueBranchResponseDto>.NotFound($"Dialogue branch with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DialogueBranchUpdateDto>(branch.MetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(branch.MetaInfo, updateDto.MetaInfo);

        if (!string.IsNullOrWhiteSpace(updateDto.Title)) branch.Title = updateDto.Title;
        if (!string.IsNullOrWhiteSpace(updateDto.Slug)) branch.Slug = updateDto.Slug;
        if (updateDto.ShortDesc != null) branch.ShortDesc = updateDto.ShortDesc;
        if (updateDto.Status.HasValue) branch.Status = (ContentStatusEnum)updateDto.Status.Value;

        await _db.SaveChangesAsync();
        return ApiResponseDto<DialogueBranchResponseDto>.Success(CreateResponseDto(branch));
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteBranchAsync(Guid id)
    {
        var branch = await _db.DialogueBranches.Include(db => db.MetaInfo).FirstOrDefaultAsync(db => db.Id == id);
        if (branch is null) return ApiResponseDto<DeleteResponseDto>.NotFound($"Dialogue branch with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(branch.MetaInfo.ProjectId);
        if (error != null) return error;

        // Cascade delete: remove all nodes in this branch first, then the branch itself
        var allNodes = await _db.DialogueNodes.Where(n => n.BranchId == id).ToListAsync();
        foreach (var node in allNodes)
            _db.Database.ExecuteSqlInterpolated($"DELETE FROM \"dialoguenodes\" WHERE \"id\" = '{node.Id}'");

        // Also delete the root node if it exists
        var rootNode = await _db.DialogueNodes.FirstOrDefaultAsync(n => n.Id == branch.RootNodeId);
        if (rootNode != null)
            _db.Database.ExecuteSqlInterpolated($"DELETE FROM \"dialoguenodes\" WHERE \"id\" = '{rootNode.Id}'");

        _db.MetaInfos.Remove(branch.MetaInfo);
        _db.DialogueBranches.Remove(branch);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto { EntityId = id, ProjectId = branch.MetaInfo.ProjectId });
    }

    // ========================================================================
    // HELPER: Get or create root node for a branch
    // ========================================================================

    private async Task<DialogueNode> GetOrCreateRootNodeAsync(Guid projectId, Guid? nodeId)
    {
        if (nodeId.HasValue)
        {
            var existing = await _db.DialogueNodes.FirstOrDefaultAsync(n => n.Id == nodeId.Value);
            return existing ?? new DialogueNode
            {
                Id = nodeId.Value,
                BranchId = null!, // Will be set after branch creation
                Text = "",
                ChoiceOptions = Enumerable.Empty<DialogueChoiceOption>(),
                NextNodeId = null,
            };
        }

        var node = new DialogueNode
        {
            Id = Guid.NewGuid(),
            BranchId = null!, // Set after branch is saved
            Text = "",
            ChoiceOptions = new List<DialogueChoiceOption>(),
            NextNodeId = null,
        };

        _db.DialogueNodes.Add(node);
        await _db.SaveChangesAsync();

        return node;
    }
}