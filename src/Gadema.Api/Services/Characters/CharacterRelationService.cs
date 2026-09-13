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
/// Handles relationship types, strength tracking, and bidirectional relations.
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
            MetaInfoId = relation.MetaInfoId,
            RelationType = (int)relation.RelationType,
            Strength = relation.Strength,
            Description = relation.Description,
            CharacterAId = relation.CharacterAId,
            CharacterBId = relation.CharacterBId,
            CreatedAt = relation.CreatedAt,
        };

    // ========================================================================
    // GET - List all character relations for a project
    // ========================================================================

        public Task<ApiResponseDto<IEnumerable<CharacterRelationResponseDto>>> GetRelationsAsync(Guid projectId)
    {
        var error = ValidateProjectAccessAsync<IEnumerable<CharacterRelationResponseDto>>(projectId);
        if (error != null) return Task.FromResult(error);

        var relations = _db.CharacterRelations
            .Include(cr => cr.MetaInfo)
            .Where(cr => cr.MetaInfo.ProjectId == projectId)
            .OrderBy(cr => cr.RelationType).ThenBy(cr => cr.Strength)
            .Select(CreateResponseDto)
            .ToListAsync();

        return Task.FromResult(ApiResponseDto<IEnumerable<CharacterRelationResponseDto>>.Success(relations));
    }

    public async Task<ApiResponseDto<CharacterRelationResponseDto>> GetRelationByIdAsync(Guid id)
    {
        var relation = await _db.CharacterRelations.Include(cr => cr.MetaInfo).FirstOrDefaultAsync(cr => cr.Id == id);

        if (relation is null)
            return ApiResponseDto<CharacterRelationResponseDto>.NotFound($"Character relation with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<CharacterRelationResponseDto>(relation.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<CharacterRelationResponseDto>.Success(CreateResponseDto(relation));
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateCharacterRelationAsync(Guid projectId, CharacterRelationCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var metaInfo = CreateMetaInfo(projectId, ContentTypeEnum.CharacterRelation, createDto.CreateData);

        _db.MetaInfos.Add(metaInfo);
        await _db.SaveChangesAsync();

        // Ensure bidirectional relation exists
        var existing = await _db.CharacterRelations.FirstOrDefaultAsync(
            cr => (cr.CharacterAId == createDto.CharacterAId && cr.CharacterBId == createDto.CharacterBId) ||
                  (cr.CharacterAId == createDto.CharacterBId && cr.CharacterBId == createDto.CharacterAId));

        if (existing != null) return ApiResponseDto<CreateResponseDto>.Conflict("Relation already exists.");

        var relation = new CharacterRelation
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id.Value,
            RelationType = (CharacterRelationTypeEnum)createDto.RelationType,
            Strength = createDto.Strength ?? 50,
            Description = createDto.Description,
            CharacterAId = createDto.CharacterAId,
            CharacterBId = createDto.CharacterBId,
        };

        _db.CharacterRelations.Add(relation);
        await _db.SaveChangesAsync();

        // Create reverse relation
        var reverseRelation = new CharacterRelation
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id.Value,
            RelationType = createDto.RelationType.Reverse(),
            Strength = createDto.Strength ?? 50,
            Description = createDto.Description,
            CharacterAId = createDto.CharacterBId,
            CharacterBId = createDto.CharacterAId,
        };

        _db.CharacterRelations.Add(reverseRelation);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = relation.Id,
            MetaInfoId = metaInfo.Id.Value,
            ProjectId = projectId
        });
    }

    public async Task<ApiResponseDto<CharacterRelationResponseDto>> UpdateCharacterRelationAsync(Guid id, CharacterRelationUpdateDto updateDto)
    {
        var relation = await _db.CharacterRelations.Include(cr => cr.MetaInfo).FirstOrDefaultAsync(cr => cr.Id == id);

        if (relation is null)
            return ApiResponseDto<CharacterRelationResponseDto>.NotFound($"Character relation with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<CharacterRelationUpdateDto>(relation.MetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(relation.MetaInfo, updateDto.MetaInfo);

        if (updateDto.RelationType.HasValue) relation.RelationType = (CharacterRelationTypeEnum)updateDto.RelationType.Value;
        if (updateDto.Strength.HasValue) relation.Strength = updateDto.Strength.Value;
        if (!string.IsNullOrWhiteSpace(updateDto.Description)) relation.Description = updateDto.Description;

        await _db.SaveChangesAsync();

        return ApiResponseDto<CharacterRelationResponseDto>.Success(CreateResponseDto(relation));
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteCharacterRelationAsync(Guid id)
    {
        var relation = await _db.CharacterRelations.Include(cr => cr.MetaInfo).FirstOrDefaultAsync(cr => cr.Id == id);

        if (relation is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Character relation with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(relation.MetaInfo.ProjectId);
        if (error != null) return error;

        _db.CharacterRelations.Remove(relation);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = relation.MetaInfo.ProjectId
        });
    }
}