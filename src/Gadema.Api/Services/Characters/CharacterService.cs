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
     .Include(c => c.StoryProfile)
     .Include(c => c.CurrentState).ThenInclude(cs => cs!.Faction)
     .Include(c => c.CurrentState).ThenInclude(cs => cs!.Location)
     .Where(c => c.MetaInfo.ProjectId == projectId)
     .OrderBy(c => c.Name)
     .ThenBy(c => c.MetaInfo.Title)
     .Select(c => CreateResponseDto(c))
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
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Faction)
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Location)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (character is null)
            return ApiResponseDto<CharacterResponseDto>.NotFound($"Character with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<CharacterResponseDto>(character.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<CharacterResponseDto>.Success(CreateResponseDto(character));
    }

    // ========================================================================
    // POST - Create a new character
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateCharacterAsync(Guid projectId, CharacterCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;


        // Create MetaInfo for the character identity
        var metaInfo = CreateMetaInfo(projectId, ContentTypeEnum.Character, createDto.CreateData);

        _db.MetaInfos.Add(metaInfo);
        await _db.SaveChangesAsync();

        // Create the Character glue entity
        var character = new Character
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id,
            Name = createDto.Name,
            NickName = createDto.NickName,
            StoryProfileId = createDto.StoryProfileId,
            CurrentStateId = null
        };

        _db.Characters.Add(character);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = character.Id,
            MetaInfoId = metaInfo.Id,
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
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Faction)
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Location)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (character is null)
            return ApiResponseDto<CharacterResponseDto>.NotFound($"Character with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<CharacterResponseDto>(character.MetaInfo.ProjectId);
        if (error != null) return error;

        if (updateDto.Name != null)
            character.Name = updateDto.Name;

        if (updateDto.NickName != null)
            character.NickName = updateDto.NickName;

        if (updateDto.StoryProfileId.HasValue)
        {
            character.StoryProfileId = updateDto.StoryProfileId.Value;
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
        return ApiResponseDto<CharacterResponseDto>.Success(CreateResponseDto(character));
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

    private CharacterResponseDto CreateResponseDto(Character character)
    {
        return new CharacterResponseDto()
        {
            Id = character.Id,
            MetaInfoId = character.MetaInfoId,
            MetaInfoTitle = character.MetaInfo.Title,
            Status = character.MetaInfo.Status,
            IsPublic = character.MetaInfo.IsPublic,
            CreatedAt = character.MetaInfo.CreatedAt,
            LastModifiedAt = character.MetaInfo.LastModifiedAt,
            CurrentStateId = character.CurrentStateId,
            StoryProfileId = character.StoryProfileId,
            Name = character.Name,
            NickName = character.NickName
        };
    }
}
