using Gadema.Api.CoreServices;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Writing.DialogueTrees;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Core.Models.Writing.Narrative;
using Gadema.Data.Database.Core;
using Gadema.Data.Database.Writing;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.Narrative;

[ServiceLifetime(ServiceLifetime.Scoped)] public class DialogueNodeService : DomainService
{
    private WritingDbContext _db;

    public DialogueNodeService( WritingDbContext db,
        ILogger<DialogueNodeService> logger,  
        CoreServicesProvider coreServices) // Injected via CoreService constructor
        : base(coreServices,logger) {_db = db; }

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
        var error = await CheckAccessAsync<IEnumerable<DialogueNodeResponseDto>>(projectId, Permission.CanView);
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

    public async Task<ApiResponseDto<DialogueNodeResponseDto>> GetNodeAsync(Guid id)
    {
        var node = await _db.DialogueNodes
            .Include(n => n.DialogueBranch).ThenInclude(b => b.ContentMetaInfo)
            .Include(n => n.Speaker).ThenInclude(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (node is null) return ApiResponseDto<DialogueNodeResponseDto>.NotFound($"Node {id} not found.");

        var error = await CheckAccessAsync<DialogueNodeResponseDto>(node.DialogueBranch.ContentMetaInfo.ProjectId, Permission.CanView);
        if (error != null) return error;

        return ApiResponseDto<DialogueNodeResponseDto>.Success(MapToResponseDto(node));
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateNodeAsync(Guid projectId, Guid branchId, DialogueNodeCreateDto createDto)
    {
        var error = await CheckAccessAsync<CreateResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        var branchResult = await GetBranchAndValidateAsync(projectId, branchId);
        if (!branchResult.Successful) return ApiResponseDto<CreateResponseDto>.BadRequest(branchResult.Message);

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
        var node = await _db.DialogueNodes.FirstOrDefaultAsync(n => n.Id == nodeId);
        if (node is null) return ApiResponseDto<DialogueNodeResponseDto>.NotFound($"Node {nodeId} not found.");

        if (node.DialogueBranchId != branchId)
            return ApiResponseDto<DialogueNodeResponseDto>.BadRequest("The node does not belong to the specified branch.");

        var error = await CheckAccessAsync<DialogueNodeResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        if (updateDto.NodeText != null) node.NodeText = updateDto.NodeText;
        if (updateDto.SpeakerId.HasValue) node.SpeakerId = updateDto.SpeakerId;
        if (updateDto.ParentNodeId.HasValue) node.ParentNodeId = updateDto.ParentNodeId;
        if (updateDto.ChoiceOptions != null) node.ChoiceOptions = updateDto.ChoiceOptions;
        if (updateDto.Conditions != null) node.Conditions = updateDto.Conditions;

        await _db.SaveChangesAsync();
        return ApiResponseDto<DialogueNodeResponseDto>.Success(MapToResponseDto(node));
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteNodeAsync(Guid id)
    {
        var node = await _db.DialogueNodes
            .Include(n => n.DialogueBranch)
                .ThenInclude(b => b.ContentMetaInfo)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (node is null) return ApiResponseDto<DeleteResponseDto>.NotFound($"Node {id} not found.");

        var error = await CheckAccessAsync<DeleteResponseDto>(node.DialogueBranch.ContentMetaInfo.ProjectId, Permission.CanDelete);
        if (error != null) return error;

        _db.DialogueNodes.Remove(node);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto 
        { 
            EntityId = id, 
            ProjectId = node.DialogueBranch.ContentMetaInfo.ProjectId.Value 
        });
    }
}