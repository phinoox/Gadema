using Gadema.Core.Dtos;
using Gadema.Core.Dtos.DialogueTrees;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Enums;
using Gadema.Core.Models;

using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.DialogueTrees;

public class DialogueBranchService : CoreService
{
    public DialogueBranchService(GameDbContext db, ILogger<DialogueBranchService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    public async Task<ApiResponseDto<ListResponseDto<DialogueBranchResponseDto>>> GetBranchesAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<DialogueBranchResponseDto>>(projectId);
        if (error != null) return error;

        var branches = await _db.DialogueBranches
            .Where(b => b.MetaInfo.ProjectId == projectId)
            .OrderByDescending(b => b.MetaInfo.CreatedAt)
            .Select(b => new DialogueBranchResponseDto
            {
                Id = b.Id,
                MetaInfoId = b.MetaInfoId.Value,
                Title = b.MetaInfo.Title,
                Slug = b.MetaInfo.Slug,
                Status = b.MetaInfo.Status,
                CreatedAt = b.MetaInfo.CreatedAt
            })
            .ToListAsync();

        return ApiResponseDto<ListResponseDto<DialogueBranchResponseDto>>.Success(new ListResponseDto<DialogueBranchResponseDto>
        {
            Items = branches,
            TotalCount = branches.Count
        });
    }

    public async Task<ApiResponseDto<DialogueBranchResponseDto>> GetBranchAsync(Guid id)
    {
        var branch = await _db.DialogueBranches
            .Include(b => b.MetaInfo)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (branch == null) return ApiResponseDto<DialogueBranchResponseDto>.NotFound("Dialogue branch not found.");

        var error = await ValidateProjectAccessAsync<DialogueBranchResponseDto>(branch.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<DialogueBranchResponseDto>.Success(new DialogueBranchResponseDto
        {
            Id = branch.Id,
            MetaInfoId = branch.MetaInfoId.Value,
            Title = branch.MetaInfo.Title,
            Slug = branch.MetaInfo.Slug,
            Status = branch.MetaInfo.Status,
            CreatedAt = branch.MetaInfo.CreatedAt
        });
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateBranchAsync(Guid projectId, DialogueBranchCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var metaInfo = CreateMetaInfo(projectId, ContentTypeEnum.Scene, createDto.CreateData);

        _db.MetaInfos.Add(metaInfo);
        await _db.SaveChangesAsync();

        var branch = new DialogueBranch
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id,
            // Add branch-specific fields here if needed
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

    public async Task<ApiResponseDto<DialogueBranchResponseDto>> UpdateBranchAsync(Guid id, DialogueBranchUpdateDto updateDto)
    {
        var branch = await _db.DialogueBranches
            .Include(b => b.MetaInfo)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (branch == null) return ApiResponseDto<DialogueBranchResponseDto>.NotFound("Dialogue branch not found.");

        var error = await ValidateProjectAccessAsync<DialogueBranchResponseDto>(branch.MetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(branch.MetaInfo, updateDto.MetaInfo);
        // Update branch-specific fields here if needed

        await _db.SaveChangesAsync();

        return ApiResponseDto<DialogueBranchResponseDto>.Success(new DialogueBranchResponseDto
        {
            Id = branch.Id,
            MetaInfoId = branch.MetaInfoId.Value,
            Title = branch.MetaInfo.Title,
            Slug = branch.MetaInfo.Slug,
            Status = branch.MetaInfo.Status,
            CreatedAt = branch.MetaInfo.CreatedAt
        });
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteBranchAsync(Guid id)
    {
        var branch = await _db.DialogueBranches
            .Include(b => b.MetaInfo)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (branch == null) return ApiResponseDto<DeleteResponseDto>.NotFound("Dialogue branch not found.");

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