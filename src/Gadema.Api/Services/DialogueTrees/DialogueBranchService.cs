using Gadema.Core.Dtos;
using Gadema.Core.Dtos.DialogueTrees;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Enums;
using Gadema.Core.Models;

using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.Narrative;

public class DialogueBranchService : CoreService
{
    public DialogueBranchService(GameDbContext db, ILogger<DialogueBranchService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    public async Task<ApiResponseDto<ListResponseDto<DialogueBranchResponseDto>>> GetBranchesAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<DialogueBranchResponseDto>>(projectId);
        if (error != null) return error;

        var branches = await _db.DialogueBranches
            .Where(b => b.ContentMetaInfo.ProjectId == projectId)
            .OrderByDescending(b => b.ContentMetaInfo.CreatedAt)
            .Select(b => new DialogueBranchResponseDto
            {
                Id = b.Id,
                MetaInfoId = b.MetaInfoId.Value,
                Title = b.ContentMetaInfo.Title,
                Slug = b.ContentMetaInfo.Slug,
                Status = b.ContentMetaInfo.Status,
                CreatedAt = b.ContentMetaInfo.CreatedAt
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
            .Include(b => b.ContentMetaInfo)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (branch == null) return ApiResponseDto<DialogueBranchResponseDto>.NotFound("Dialogue branch not found.");

        var error = await ValidateProjectAccessAsync<DialogueBranchResponseDto>(branch.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<DialogueBranchResponseDto>.Success(new DialogueBranchResponseDto
        {
            Id = branch.Id,
            MetaInfoId = branch.MetaInfoId.Value,
            Title = branch.ContentMetaInfo.Title,
            Slug = branch.ContentMetaInfo.Slug,
            Status = branch.ContentMetaInfo.Status,
            CreatedAt = branch.ContentMetaInfo.CreatedAt
        });
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateBranchAsync(Guid projectId, DialogueBranchCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var ContentMetaInfo = CreateMetaInfo(projectId, ContentTypeEnum.Scene, createDto.CreateData);

        _db.MetaInfos.Add(ContentMetaInfo);
        await _db.SaveChangesAsync();

        var branch = new DialogueBranch
        {
            Id = Guid.NewGuid(),
            MetaInfoId = ContentMetaInfo.Id,
            // Add branch-specific fields here if needed
        };

        _db.DialogueBranches.Add(branch);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = branch.Id,
            MetaInfoId = ContentMetaInfo.Id,
            ProjectId = projectId
        });
    }

    public async Task<ApiResponseDto<DialogueBranchResponseDto>> UpdateBranchAsync(Guid id, DialogueBranchUpdateDto updateDto)
    {
        var branch = await _db.DialogueBranches
            .Include(b => b.ContentMetaInfo)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (branch == null) return ApiResponseDto<DialogueBranchResponseDto>.NotFound("Dialogue branch not found.");

        var error = await ValidateProjectAccessAsync<DialogueBranchResponseDto>(branch.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(branch.ContentMetaInfo, updateDto.ContentMetaInfo);
        // Update branch-specific fields here if needed

        await _db.SaveChangesAsync();

        return ApiResponseDto<DialogueBranchResponseDto>.Success(new DialogueBranchResponseDto
        {
            Id = branch.Id,
            MetaInfoId = branch.MetaInfoId.Value,
            Title = branch.ContentMetaInfo.Title,
            Slug = branch.ContentMetaInfo.Slug,
            Status = branch.ContentMetaInfo.Status,
            CreatedAt = branch.ContentMetaInfo.CreatedAt
        });
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteBranchAsync(Guid id)
    {
        var branch = await _db.DialogueBranches
            .Include(b => b.ContentMetaInfo)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (branch == null) return ApiResponseDto<DeleteResponseDto>.NotFound("Dialogue branch not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(branch.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        _db.MetaInfos.Remove(branch.ContentMetaInfo);
        _db.DialogueBranches.Remove(branch);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = branch.ContentMetaInfo.ProjectId
        });
    }
}