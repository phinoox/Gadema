// ... existing imports ...

using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Writing.DialogueTrees;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Writing.Narrative;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.Narrative;

[ServiceLifetime(ServiceLifetime.Scoped)] public class DialogueNodeService : CoreService
{
    public DialogueNodeService(GameDbContext db, ILogger<DialogueNodeService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    #region Mapping Helpers

    private DialogueNodeResponseDto MapToResponseDto(DialogueNode node)
        => new()
        {
            Id = node.Id,
            DialogueBranchId = node.DialogueBranchId,
            BranchTitle = node.DialogueBranch?.ContentMetaInfo?.Title ?? "Unknown Branch",
            NodeText = node.NodeText,
            SpeakerId = node.SpeakerId,
            SpeakerName = node.Speaker?.ContentMetaInfo?.Title, 
            ChoiceOptions = node.ChoiceOptions,
            Conditions = node.Conditions,
            ParentNodeId = node.ParentNodeId,
        };

    #endregion

    #region Internal Helpers

    private async Task<ApiResponseDto<DialogueBranch>> GetBranchAndValidateAsync(Guid projectId, Guid branchId)
    {
        var branch = await _db.DialogueBranches
            .Include(b => b.ContentMetaInfo)
            .FirstOrDefaultAsync(b => b.Id == branchId && b.ContentMetaInfo.ProjectId == projectId);

        if (branch == null) 
            return ApiResponseDto<DialogueBranch>.BadRequest("Branch not found or access denied.");

        return ApiResponseDto<DialogueBranch>.Success(branch);
    }

    #endregion

    public async Task<ApiResponseDto<IEnumerable<DialogueNodeResponseDto>>> GetNodesAsync(Guid projectId, Guid? branchId = null)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<DialogueNodeResponseDto>>(projectId);
        if (error != null) return error;

        var query = _db.DialogueNodes
            .Include(n => n.DialogueBranch).ThenInclude(b => b.ContentMetaInfo)
            .Include(n => n.Speaker).ThenInclude(s => s.ContentMetaInfo)
            .Where(n => n.DialogueBranch.ContentMetaInfo.ProjectId == projectId);

        if (branchId.HasValue) 
            query = query.Where(n => n.DialogueBranchId == branchId.Value);

        var nodes = await query.ToListAsync();
        return ApiResponseDto<IEnumerable<DialogueNodeResponseDto>>.Success(nodes.Select(MapToResponseDto));
    }

    public async Task<ApiResponseDto<DialogueNodeResponseDto>> GetNodeByIdAsync(Guid id)
    {
        var node = await _db.DialogueNodes
            .Include(n => n.DialogueBranch).ThenInclude(b => b.ContentMetaInfo)
            .Include(n => n.Speaker).ThenInclude(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (node is null) return ApiResponseDto<DialogueNodeResponseDto>.NotFound($"Node {id} not found.");

        var error = await ValidateProjectAccessAsync<DialogueNodeResponseDto>(node.DialogueBranch.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<DialogueNodeResponseDto>.Success(MapToResponseDto(node));
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateNodeAsync(Guid projectId, Guid branchId, DialogueNodeCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        // Use the helper to validate and fetch the branch in one go
        var branchResult = await GetBranchAndValidateAsync(projectId, branchId);
        if (!branchResult.Successful) return ApiResponseDto<CreateResponseDto>.BadRequest(branchResult.Message);
        var branch = branchResult;

        var node = new DialogueNode
        {
            Id = Guid.NewGuid(),
            DialogueBranchId = branchId,
            NodeText = createDto.NodeText,
            SpeakerId = createDto.SpeakerId,
            ParentNodeId = createDto.ParentNodeId,
            ChoiceOptions = createDto.ChoiceOptions,
            Conditions = createDto.Conditions
        };

        _db.DialogueNodes.Add(node);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto 
        { 
            EntityId = node.Id, 
            ProjectId = projectId 
        });
    }

    public async Task<ApiResponseDto<DialogueNodeResponseDto>> UpdateNodeAsync(Guid projectId, Guid branchId, Guid nodeId, DialogueNodeUpdateDto updateDto)
    {
        // 1. Fetch the node
        var node = await _db.DialogueNodes.FirstOrDefaultAsync(n => n.Id == nodeId);
        if (node is null) return ApiResponseDto<DialogueNodeResponseDto>.NotFound($"Node {nodeId} not found.");

        // 2. Verify Contextual Integrity: Does this node actually belong to the branch in the URL?
        if (node.DialogueBranchId != branchId)
            return ApiResponseDto<DialogueNodeResponseDto>.BadRequest("The node does not belong to the specified branch.");

        // 3. Validate Project Access via the branch's ContentMetaInfo
        var error = await ValidateProjectAccessAsync<DialogueNodeResponseDto>(projectId);
        if (error != null) return error;

        // 4. Apply updates to content
        ApplyUpdateToModel(node, updateDto);

        await _db.SaveChangesAsync();
        return ApiResponseDto<DialogueNodeResponseDto>.Success(MapToResponseDto(node));
    }
    private void ApplyUpdateToModel(DialogueNode node, DialogueNodeUpdateDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.NodeText)) node.NodeText = dto.NodeText;
        if (dto.SpeakerId.HasValue) node.SpeakerId = dto.SpeakerId;
        if (dto.ParentNodeId.HasValue) node.ParentNodeId = dto.ParentNodeId;
        if (dto.ChoiceOptions != null) node.ChoiceOptions = dto.ChoiceOptions;
        if (dto.Conditions != null) node.Conditions = dto.Conditions;
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteNodeAsync(Guid id)
    {
        var node = await _db.DialogueNodes.FirstOrDefaultAsync(n => n.Id == id);
        if (node is null) return ApiResponseDto<DeleteResponseDto>.NotFound($"Node {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(node.DialogueBranch.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        _db.DialogueNodes.Remove(node);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto 
        { 
            EntityId = id, 
            ProjectId = node.DialogueBranch.ContentMetaInfo.ProjectId 
        });
    }
}