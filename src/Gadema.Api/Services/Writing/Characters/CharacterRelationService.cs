using Gadema.Api.CoreServices;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Writing.Characters;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Core.Models.Writing.Characters;
using Gadema.Data.Database.Core;
using Gadema.Data.Database.Writing;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.Characters;

/// <summary>
/// Service for managing Character Relations - connections between characters.
/// Access is validated via the TriggerScene's project ownership.
/// </summary>
[ServiceLifetime(ServiceLifetime.Scoped)] public class CharacterRelationService : DomainService
{
    private WritingDbContext _db;

    public CharacterRelationService( WritingDbContext db,
        ILogger<CharacterRelationService> logger,  
        ICoreServicesProvider coreServices) // Injected via CoreService constructor
        : base(coreServices,logger) { _db = db; }

    /// <summary>Creates a response DTO from a CharacterRelation entity.</summary>
    private CharacterRelationResponseDto CreateResponseDto(CharacterRelation relation)
        => new()
        {
            Id = relation.Id,
            RelationType = relation.RelationType,
            Description = relation.Description,
            SourceCharacterId = relation.SourceCharacterId,
            TargetCharacterId = relation.TargetCharacterId,
            TriggerSceneId = relation.TriggerSceneId,
            CreatedAt = relation.CreatedAt
        };

    // ========================================================================
    // GET - List all character relations for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<CharacterRelationResponseDto>>> GetRelationsAsync(Guid projectId)
    {
        // 1. Check permission to view the project scope
        var error = await CheckAccessAsync<IEnumerable<CharacterRelationResponseDto>>(projectId, Permission.CanView);
        if (error != null) return error;

        var relations = await _db.CharacterRelations
            .Include(cr => cr.TriggerScene)
                .ThenInclude(s => s.ContentMetaInfo)
            .Where(cr => cr.TriggerScene.ContentMetaInfo.ProjectId == projectId)
            .OrderBy(cr => cr.RelationType)
            .ToListAsync();

        var projected = relations.Select(CreateResponseDto).ToList();
        return ApiResponseDto<IEnumerable<CharacterRelationResponseDto>>.Success(projected);
    }

    public async Task<ApiResponseDto<CharacterRelationResponseDto>> GetRelationByIdAsync(Guid id)
    {
        var relation = await _db.CharacterRelations
            .Include(cr => cr.TriggerScene)
                .ThenInclude(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(cr => cr.Id == id);

        if (relation is null)
            return ApiResponseDto<CharacterRelationResponseDto>.NotFound($"Character relation with ID {id} not found.");

        // 2. Validate access via the Scene's ContentMetaInfo scope
        var error = await CheckAccessAsync<CharacterRelationResponseDto>(relation.TriggerScene.ContentMetaInfo.ProjectId, Permission.CanView);
        if (error != null) return error;

        return ApiResponseDto<CharacterRelationResponseDto>.Success(CreateResponseDto(relation));
    }

    // ========================================================================
    // POST - Create a new character relation
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateCharacterRelationAsync(Guid projectId, CharacterRelationCreateDto createDto)
    {
        // 1. Check permission to edit the project scope
        var error = await CheckAccessAsync<CreateResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        // 2. Validate scene existence and ownership
        var scene = await _db.Scenes
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == createDto.TriggerSceneId && s.ContentMetaInfo.ProjectId == projectId);

        if (scene == null)
            return ApiResponseDto<CreateResponseDto>.NotFound("The specified scene does not exist in this project.");

        // 3. Verify characters belong to the same project
        var charAExists = await _db.Characters.AnyAsync(c => c.Id == createDto.SourceCharacterId && c.ContentMetaInfo.ProjectId == projectId);
        var charBExists = await _db.Characters.AnyAsync(c => c.Id == createDto.TargetCharacterId && c.ContentMetaInfo.ProjectId == projectId);

        if (!charAExists || !charBExists)
            return ApiResponseDto<CreateResponseDto>.BadRequest("One or both characters do not belong to this project.");

        // 4. Check for existing relation (to prevent duplicates)
        var exists = await _db.CharacterRelations.AnyAsync(cr => 
            cr.SourceCharacterId == createDto.SourceCharacterId && 
            cr.TargetCharacterId == createDto.TargetCharacterId &&
            cr.TriggerSceneId == createDto.TriggerSceneId);

        if (exists) return ApiResponseDto<CreateResponseDto>.Conflict("This specific relation already exists in this scene.");

        // 5. Create the entity
        var relation = new CharacterRelation
        {
            Id = Guid.NewGuid(),
            SourceCharacterId = createDto.SourceCharacterId,
            TargetCharacterId = createDto.TargetCharacterId,
            RelationType = createDto.RelationType,
            TriggerSceneId = createDto.TriggerSceneId,
            Description = createDto.Description,
            CreatedAt = DateTime.UtcNow
        };

        _db.CharacterRelations.Add(relation);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = relation.Id,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a character relation
    // ========================================================================

    public async Task<ApiResponseDto<CharacterRelationResponseDto>> UpdateCharacterRelationAsync(Guid id, CharacterRelationUpdateDto updateDto)
    {
        var relation = await _db.CharacterRelations
            .Include(cr => cr.TriggerScene)
                .ThenInclude(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(cr => cr.Id == id);

        if (relation is null)
            return ApiResponseDto<CharacterRelationResponseDto>.NotFound($"Character relation with ID {id} not found.");

        // 1. Validate access via the Scene's ContentMetaInfo scope
        var error = await CheckAccessAsync<CharacterRelationResponseDto>(relation.TriggerScene.ContentMetaInfo.ProjectId, Permission.CanEdit);
        if (error != null) return error;

        // 2. Update properties if provided in DTO
        relation.RelationType = updateDto.RelationType;
        if (updateDto.Description != null) relation.Description = updateDto.Description;
        relation.TriggerSceneId = updateDto.TriggerSceneId;

        await _db.SaveChangesAsync();

        return ApiResponseDto<CharacterRelationResponseDto>.Success(CreateResponseDto(relation));
    }

    // ========================================================================
    // DELETE - Remove a character relation
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteCharacterRelationAsync(Guid id)
    {
        var relation = await _db.CharacterRelations
            .Include(cr => cr.TriggerScene)
                .ThenInclude(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(cr => cr.Id == id);

        if (relation is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Character relation with ID {id} not found.");

        // 1. Validate access via the Scene's ContentMetaInfo scope
        var error = await CheckAccessAsync<DeleteResponseDto>(relation.TriggerScene.ContentMetaInfo.ProjectId, Permission.CanDelete);
        if (error != null) return error;

        _db.CharacterRelations.Remove(relation);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = relation.TriggerScene.ContentMetaInfo.ProjectId.Value
        });
    }
}