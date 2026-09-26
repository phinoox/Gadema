using Gadema.Api.CoreServices;
using Gadema.Api.CoreServices.Strategies;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Writing.DialogueTrees;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Core.Models.Writing.Narrative;
using Gadema.Data.Database.Core;
using Gadema.Data.Database.Writing;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.Narrative;

[ServiceLifetime(ServiceLifetime.Scoped)] public class DialogueBranchService : DomainService
{
    private WritingDbContext _db;

    public DialogueBranchService( WritingDbContext db,
        ILogger<DialogueBranchService> logger,  
        CoreServicesProvider coreServices) // Injected via CoreService constructor
        : base(coreServices,logger) {_db = db; }

    // ========================================================================
    // GET - List all branches for a project
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<DialogueBranchResponseDto>>> GetBranchesAsync(Guid projectId)
    {
        var error = await CheckAccessAsync<ListResponseDto<DialogueBranchResponseDto>>(projectId, Permission.CanView);
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

        var error = await CheckAccessAsync<DialogueBranchResponseDto>(branch.ContentMetaInfo.ProjectId, Permission.CanView);
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

    // ========================================================================
    // POST - Create a new branch
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateBranchAsync(Guid projectId, DialogueBranchCreateDto createDto)
    {
        var error = await CheckAccessAsync<CreateResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        var contentMetaInfo = await _core.MetadataService.CreateAsync<ContentMetaInfo>(createDto.CreateData, m =>
        {
            m.ProjectId = projectId;
            m.ContentType = ContentTypeEnum.Scene;
        });

        var branch = new DialogueBranch
        {
            Id = Guid.NewGuid(),
            MetaInfoId = contentMetaInfo.Id,
        };

        _db.DialogueBranches.Add(branch);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = branch.Id,
            MetaInfoId = contentMetaInfo.Id,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a branch
    // ========================================================================

    public async Task<ApiResponseDto<DialogueBranchResponseDto>> UpdateBranchAsync(Guid id, DialogueBranchUpdateDto updateDto)
    {
        var branch = await _db.DialogueBranches
            .Include(b => b.ContentMetaInfo)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (branch == null) return ApiResponseDto<DialogueBranchResponseDto>.NotFound("Dialogue branch not found.");

        var error = await CheckAccessAsync<DialogueBranchResponseDto>(branch.ContentMetaInfo.ProjectId, Permission.CanEdit);
        if (error != null) return error;

        if (updateDto.ContentMetaInfo != null)
        {
            var success = await SyncIdentityAsync<ContentIdentityStrategy>(branch.MetaInfoId.Value, updateDto.ContentMetaInfo);
            if (!success) return ApiResponseDto<DialogueBranchResponseDto>.ServerError("Sync failed.");
        }

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

    // ========================================================================
    // DELETE - Remove a branch
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteBranchAsync(Guid id)
    {
        var branch = await _db.DialogueBranches
            .Include(b => b.ContentMetaInfo)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (branch == null) return ApiResponseDto<DeleteResponseDto>.NotFound("Dialogue branch not found.");

        var error = await CheckAccessAsync<DeleteResponseDto>(branch.ContentMetaInfo.ProjectId, Permission.CanDelete);
        if (error != null) return error;

        
        _db.DialogueBranches.Remove(branch);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = branch.ContentMetaInfo.ProjectId.Value
        });
    }
}