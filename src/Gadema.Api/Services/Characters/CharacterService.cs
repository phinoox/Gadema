// src/Gadema.Api/Services/Characters/CharacterService.cs
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Characters;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Models.Characters;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Characters;

public class CharacterService : CoreService
{
    public CharacterService(GameDbContext db, ILogger<CharacterService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List all characters for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<CharacterResponseDto>>> GetCharactersAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<CharacterResponseDto>>(projectId);
        if (error != null) return error;

        var characters = await _db.Characters
            .Include(c => c.MetaInfo)
            .Include(c => c.CharacterDetails)
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Faction)
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Location)
            .Where(c => c.MetaInfo.ProjectId == projectId)
            .OrderBy(c => c.MetaInfo.Title)
            .Select(c => new CharacterResponseDto
            {
                Id = c.Id,
                MetaInfoId = c.MetaInfoId,
                MetaInfoTitle = c.MetaInfo.Title,
                Status = c.MetaInfo.Status,
                IsPublic = c.MetaInfo.IsPublic,
                CreatedAt = c.MetaInfo.CreatedAt,
                LastModifiedAt = c.MetaInfo.LastModifiedAt,
                CharacterDetailsId = c.CharacterDetailsId,
                Details = new CharacterDetailsResponseDto
                {
                    Id = c.CharacterDetails.Id,
                    MetaInfoId = c.CharacterDetails.MetaInfoId,
                    Name = c.CharacterDetails.Name,
                    ClassTemplateId = c.CharacterDetails.ClassTemplateId,
                    Level = c.CharacterDetails.Level,
                    Role = c.CharacterDetails.Role,
                    Status = c.CharacterDetails.Status
                },
                CurrentStateId = c.CurrentStateId,
                CurrentStateData = c.CurrentState != null ? new CharacterStateResponseDto
                {
                    Id = c.CurrentState.Id,
                    MetaInfoId = c.CurrentState.MetaInfoId,
                    MetaInfoTitle = c.CurrentState.MetaInfo.Title,
                    Status = c.CurrentState.MetaInfo.Status,
                    IsPublic = c.CurrentState.MetaInfo.IsPublic,
                    CreatedAt = c.CurrentState.MetaInfo.CreatedAt,
                    LastModifiedAt = c.CurrentState.MetaInfo.LastModifiedAt,
                    Role = c.CurrentState.Role,
                    FactionId = c.CurrentState.FactionId,
                    FactionName = c.CurrentState.Faction != null ? c.CurrentState.Faction.MetaInfo.Title : null,
                    LocationId = c.CurrentState.LocationId,
                    LocationName = c.CurrentState.Location != null ? c.CurrentState.Location.MetaInfo.Title : null,
                    LifeStatus = c.CurrentState.LifeStatus,
                    Note = c.CurrentState.Note
                } : null
            })
            .ToListAsync();

        return ApiResponseDto<IEnumerable<CharacterResponseDto>>.Success(characters);
    }

    // ========================================================================
    // GET - Single character by ID
    // ========================================================================

    public async Task<ApiResponseDto<CharacterResponseDto>> GetCharacterAsync(Guid id)
    {
        var character = await _db.Characters
            .Include(c => c.MetaInfo)
            .Include(c => c.CharacterDetails)
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Faction)
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Location)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (character is null)
            return ApiResponseDto<CharacterResponseDto>.NotFound($"Character with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<CharacterResponseDto>(character.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<CharacterResponseDto>.Success(new CharacterResponseDto
        {
            Id = character.Id,
            MetaInfoId = character.MetaInfoId,
            MetaInfoTitle = character.MetaInfo.Title,
            Status = character.MetaInfo.Status,
            IsPublic = character.MetaInfo.IsPublic,
            CreatedAt = character.MetaInfo.CreatedAt,
            LastModifiedAt = character.MetaInfo.LastModifiedAt,
            CharacterDetailsId = character.CharacterDetailsId,
            Details = new CharacterDetailsResponseDto
            {
                Id = character.CharacterDetails.Id,
                MetaInfoId = character.CharacterDetails.MetaInfoId,
                Name = character.CharacterDetails.Name,
                ClassTemplateId = character.CharacterDetails.ClassTemplateId,
                Level = character.CharacterDetails.Level,
                Role = character.CharacterDetails.Role,
                Status = character.CharacterDetails.Status
            },
            CurrentStateId = character.CurrentStateId,
            CurrentStateData = character.CurrentState != null ? new CharacterStateResponseDto
            {
                Id = character.CurrentState.Id,
                MetaInfoId = character.CurrentState.MetaInfoId,
                MetaInfoTitle = character.CurrentState.MetaInfo.Title,
                Status = character.CurrentState.MetaInfo.Status,
                IsPublic = character.CurrentState.MetaInfo.IsPublic,
                CreatedAt = character.CurrentState.MetaInfo.CreatedAt,
                LastModifiedAt = character.CurrentState.MetaInfo.LastModifiedAt,
                Role = character.CurrentState.Role,
                FactionId = character.CurrentState.FactionId,
                FactionName = character.CurrentState.Faction != null ? character.CurrentState.Faction.MetaInfo.Title : null,
                LocationId = character.CurrentState.LocationId,
                LocationName = character.CurrentState.Location != null ? character.CurrentState.Location.MetaInfo.Title : null,
                LifeStatus = character.CurrentState.LifeStatus,
                Note = character.CurrentState.Note
            } : null
        });
    }

    // ========================================================================
    // POST - Create a new character
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateCharacterAsync(Guid projectId, CharacterCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        // Verify CharacterDetails exists and belongs to the project
        var characterDetails = await _db.CharacterDetails
            .Include(cd => cd.MetaInfo)
            .FirstOrDefaultAsync(cd => cd.Id == createDto.CharacterDetailsId);

        if (characterDetails is null)
            return ApiResponseDto<CreateResponseDto>.NotFound($"CharacterDetails with ID {createDto.CharacterDetailsId} not found.");

        if (characterDetails.MetaInfo.ProjectId != projectId)
            return ApiResponseDto<CreateResponseDto>.BadRequest("CharacterDetails does not belong to this project.");

        // Create MetaInfo for the character identity
        var metaInfo = CreateMetaInfo(projectId, ContentTypeEnum.Character, createDto.CreateData);

        _db.MetaInfos.Add(metaInfo);
        await _db.SaveChangesAsync();

        // Create the Character glue entity
        var character = new Character
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id.Value,
            CharacterDetailsId = createDto.CharacterDetailsId
        };

        _db.Characters.Add(character);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = character.Id,
            MetaInfoId = metaInfo.Id.Value,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a character's linkage
    // ========================================================================

    public async Task<ApiResponseDto<CharacterResponseDto>> UpdateCharacterAsync(Guid id, CharacterUpdateDto updateDto)
    {
        var character = await _db.Characters
            .Include(c => c.MetaInfo)
            .Include(c => c.CharacterDetails)
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Faction)
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Location)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (character is null)
            return ApiResponseDto<CharacterResponseDto>.NotFound($"Character with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<CharacterResponseDto>(character.MetaInfo.ProjectId);
        if (error != null) return error;

        // Update CharacterDetails linkage if provided
        if (updateDto.CharacterDetailsId.HasValue)
        {
            var newDetails = await _db.CharacterDetails
                .Include(cd => cd.MetaInfo)
                .FirstOrDefaultAsync(cd => cd.Id == updateDto.CharacterDetailsId.Value);

            if (newDetails is null)
                return ApiResponseDto<CharacterResponseDto>.NotFound($"CharacterDetails with ID {updateDto.CharacterDetailsId.Value} not found.");

            if (newDetails.MetaInfo.ProjectId != character.MetaInfo.ProjectId)
                return ApiResponseDto<CharacterResponseDto>.BadRequest("New CharacterDetails does not belong to this project.");

            character.CharacterDetailsId = updateDto.CharacterDetailsId.Value;
        }

        // Update CurrentState linkage if provided
        if (updateDto.CurrentStateId.HasValue)
        {
            var newState = await _db.CharacterStates
                .Include(cs => cs.MetaInfo)
                .FirstOrDefaultAsync(cs => cs.Id == updateDto.CurrentStateId.Value);

            if (newState is null)
                return ApiResponseDto<CharacterResponseDto>.NotFound($"CharacterState with ID {updateDto.CurrentStateId.Value} not found.");

            if (newState.MetaInfo.ProjectId != character.MetaInfo.ProjectId)
                return ApiResponseDto<CharacterResponseDto>.BadRequest("New CharacterState does not belong to this project.");

            character.CurrentStateId = updateDto.CurrentStateId.Value;
        }

        await _db.SaveChangesAsync();

        // Return updated response with full details
        return ApiResponseDto<CharacterResponseDto>.Success(new CharacterResponseDto
        {
            Id = character.Id,
            MetaInfoId = character.MetaInfoId,
            MetaInfoTitle = character.MetaInfo.Title,
            Status = character.MetaInfo.Status,
            IsPublic = character.MetaInfo.IsPublic,
            CreatedAt = character.MetaInfo.CreatedAt,
            LastModifiedAt = character.MetaInfo.LastModifiedAt,
            CharacterDetailsId = character.CharacterDetailsId,
            Details = new CharacterDetailsResponseDto
            {
                Id = character.CharacterDetails.Id,
                MetaInfoId = character.CharacterDetails.MetaInfoId,
                Name = character.CharacterDetails.Name,
                ClassTemplateId = character.CharacterDetails.ClassTemplateId,
                Level = character.CharacterDetails.Level,
                Role = character.CharacterDetails.Role,
                Status = character.CharacterDetails.Status
            },
            CurrentStateId = character.CurrentStateId,
            CurrentStateData = character.CurrentState != null ? new CharacterStateResponseDto
            {
                Id = character.CurrentState.Id,
                MetaInfoId = character.CurrentState.MetaInfoId,
                MetaInfoTitle = character.CurrentState.MetaInfo.Title,
                Status = character.CurrentState.MetaInfo.Status,
                IsPublic = character.CurrentState.MetaInfo.IsPublic,
                CreatedAt = character.CurrentState.MetaInfo.CreatedAt,
                LastModifiedAt = character.CurrentState.MetaInfo.LastModifiedAt,
                Role = character.CurrentState.Role,
                FactionId = character.CurrentState.FactionId,
                FactionName = character.CurrentState.Faction != null ? character.CurrentState.Faction.MetaInfo.Title : null,
                LocationId = character.CurrentState.LocationId,
                LocationName = character.CurrentState.Location != null ? character.CurrentState.Location.MetaInfo.Title : null,
                LifeStatus = character.CurrentState.LifeStatus,
                Note = character.CurrentState.Note
            } : null
        });
    }

    // ========================================================================
    // DELETE - Remove a character (and its MetaInfo, but NOT CharacterDetails)
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteCharacterAsync(Guid id)
    {
        var character = await _db.Characters
            .Include(c => c.MetaInfo)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (character is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Character with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(character.MetaInfo.ProjectId);
        if (error != null) return error;

        // Delete MetaInfo (cascades to Character from that side)
        _db.MetaInfos.Remove(character.MetaInfo);
        
        // Remove the Character record (but NOT CharacterDetails — it may be shared)
        _db.Characters.Remove(character);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = character.MetaInfo.ProjectId
        });
    }
}
