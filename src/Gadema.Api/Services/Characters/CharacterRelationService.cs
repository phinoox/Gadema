// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Characters;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Models.Characters;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Characters;

/// <summary>
/// Service for managing Character Relations - connections between characters.
/// Access is validated via the TriggerScene's project ownership.
/// </summary>
public class CharacterRelationService : CoreService
{
    public CharacterRelationService(GameDbContext db, ILogger<CharacterRelationService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

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
        // Validate access via the Scene's ContentMetaInfo
        var error = await ValidateProjectAccessAsync<IEnumerable<CharacterRelationResponseDto>>(projectId);
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

        // Validate access via the Scene's ContentMetaInfo
        var error = await ValidateProjectAccessAsync<CharacterRelationResponseDto>(relation.TriggerScene.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<CharacterRelationResponseDto>.Success(CreateResponseDto(relation));
    }

    // ========================================================================
    // POST - Create a new character relation
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateCharacterRelationAsync(Guid projectId, CharacterRelationCreateDto createDto)
    {
        // 1. Validate project access via the scene provided in DTO
        var scene = await _db.Scenes
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == createDto.TriggerSceneId && s.ContentMetaInfo.ProjectId == projectId);

        if (scene == null)
            return ApiResponseDto<CreateResponseDto>.NotFound("The specified scene does not exist in this project.");

        // 2. Verify characters belong to the same project (optional but recommended for data integrity)
        var charAExists = await _db.Characters.AnyAsync(c => c.Id == createDto.SourceCharacterId && c.ContentMetaInfo.ProjectId == projectId);
        var charBExists = await _db.Characters.AnyAsync(c => c.Id == createDto.TargetCharacterId && c.ContentMetaInfo.ProjectId == projectId);

        if (!charAExists || !charBExists)
            return ApiResponseDto<CreateResponseDto>.BadRequest("One or both characters do not belong to this project.");

        // 3. Check for existing relation (to prevent duplicates)
        var exists = await _db.CharacterRelations.AnyAsync(cr => 
            cr.SourceCharacterId == createDto.SourceCharacterId && 
            cr.TargetCharacterId == createDto.TargetCharacterId &&
            cr.TriggerSceneId == createDto.TriggerSceneId);

        if (exists) return ApiResponseDto<CreateResponseDto>.Conflict("This specific relation already exists in this scene.");

        // 4. Create the entity
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

        // Validate access via the Scene's ContentMetaInfo
        var error = await ValidateProjectAccessAsync<CharacterRelationUpdateDto>(relation.TriggerScene.ContentMetaInfo.ProjectId);
        if (error != null) return ApiResponseDto<CharacterRelationResponseDto>.Unauthorized(error.Message ?? " not authorized");

        // Update properties if provided in DTO
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

        // Validate access via the Scene's ContentMetaInfo
        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(relation.TriggerScene.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        _db.CharacterRelations.Remove(relation);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = relation.TriggerScene.ContentMetaInfo.ProjectId
        });
    }
}