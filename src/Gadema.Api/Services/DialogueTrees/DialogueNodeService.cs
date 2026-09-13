// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.DialogueTrees;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Content;

/// <summary>
/// Service for managing DialogueNodes - individual nodes within a dialogue branch.
/// Each node contains text and an optional choice that leads to the next node.
/// </summary>
public class DialogueNodeService : CoreService
{
    public DialogueNodeService(GameDbContext db, ILogger<DialogueNodeService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    private DialogueNodeResponseDto CreateResponseDto(DialogueNode node)
        => new()
        {
            Id = node.Id,
            BranchId = node.BranchId,
            Text = node.Text,
            ChoiceOptions = node.ChoiceOptions != null ? node.ChoiceOptions.Select(co => new DialogueChoiceOptionResponseDto
            {
                Key = co.Key,
                Value = co.Value,
            }).ToList() : Enumerable.Empty<DialogueChoiceOptionResponseDto>().ToList(),
            NextNodeId = node.NextNodeId,
        };

    public async Task<ApiResponseDto<IEnumerable<DialogueNodeResponseDto>>> GetNodesAsync(Guid projectId, Guid? branchId = null)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<DialogueNodeResponseDto>>(projectId);
        if (error != null) return error;

        var query = _db.DialogueNodes.Where(n => n.BranchId == null && n.MetaInfo.ProjectId == projectId).OrderBy(n => n.Id);

        if (branchId.HasValue) query = query.Where(n => n.BranchId == branchId.Value);

        var nodes = await query.ToListAsync();
        return ApiResponseDto<IEnumerable<DialogueNodeResponseDto>>.Success(nodes.Select(CreateResponseDto));
    }

    public async Task<ApiResponseDto<DialogueNodeResponseDto>> GetNodeByIdAsync(Guid id)
    {
        var node = await _db.DialogueNodes.Include(n => n.Branch).FirstOrDefaultAsync(n => n.Id == id);
        if (node is null) return ApiResponseDto<DialogueNodeResponseDto>.NotFound($"Dialogue node with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DialogueNodeResponseDto>(node.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<DialogueNodeResponseDto>.Success(CreateResponseDto(node));
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateNodeAsync(Guid projectId, DialogueNodeCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        // Find or get the branch to set BranchId
        var metaInfo = _db.MetaInfos.FirstOrDefault(m => m.Id == createDto.BranchId && m.ProjectId == projectId);
        if (metaInfo is null) return ApiResponseDto<CreateResponseDto>.BadRequest("Branch not found.");

        var node = new DialogueNode
        {
            Id = Guid.NewGuid(),
            BranchId = metaInfo.Id,
            Text = createDto.Text,
            ChoiceOptions = createDto.ChoiceOptions?.Select(co => new DialogueChoiceOption { Key = co.Key, Value = co.Value }).ToList() ?? Enumerable.Empty<DialogueChoiceOption>(),
            NextNodeId = createDto.NextNodeId,
        };

        _db.DialogueNodes.Add(node);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto { EntityId = node.Id, MetaInfoId = metaInfo.Id, ProjectId = projectId });
    }

    public async Task<ApiResponseDto<DialogueNodeResponseDto>> UpdateNodeAsync(Guid id, DialogueNodeUpdateDto updateDto)
    {
        var node = await _db.DialogueNodes.Include(n => n.Branch).FirstOrDefaultAsync(n => n.Id == id);
        if (node is null) return ApiResponseDto<DialogueNodeResponseDto>.NotFound($"Dialogue node with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DialogueNodeUpdateDto>(node.MetaInfo.ProjectId);
        if (error != null) return error;

        if (!string.IsNullOrWhiteSpace(updateDto.Text)) node.Text = updateDto.Text;

        if (updateDto.ChoiceOptions?.Any() == true)
            node.ChoiceOptions = updateDto.ChoiceOptions.Select(co => new DialogueChoiceOption { Key = co.Key, Value = co.Value }).ToList();
        else if (updateDto.ChoiceOptions != null && !updateDto.ChoiceOptions.Any())
            node.ChoiceOptions = Enumerable.Empty<DialogueChoiceOption>();

        if (!string.IsNullOrEmpty(updateDto.NextNodeId)) node.NextNodeId = updateDto.NextNodeId;

        await _db.SaveChangesAsync();
        return ApiResponseDto<DialogueNodeResponseDto>.Success(CreateResponseDto(node));
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteNodeAsync(Guid id)
    {
        var node = await _db.DialogueNodes.Include(n => n.Branch).FirstOrDefaultAsync(n => n.Id == id);
        if (node is null) return ApiResponseDto<DeleteResponseDto>.NotFound($"Dialogue node with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(node.MetaInfo.ProjectId);
        if (error != null) return error;

        _db.DialogueNodes.Remove(node);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto { EntityId = id, ProjectId = node.MetaInfo.ProjectId });
    }
}