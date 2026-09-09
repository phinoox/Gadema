// src/Gadema.Api/Services/Characters/CharacterStoryProfileService.cs
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Characters;
using Gadema.Core.Models;
using Gadema.Core.Models.Characters;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Characters;

public class CharacterStoryProfileService : CoreService
{
    public CharacterStoryProfileService(GameDbContext db, ILogger<CharacterStoryProfileService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - Get story profile for a character
    // ========================================================================

    public async Task<ApiResponseDto<CharacterStoryProfileResponseDto>> GetStoryProfileAsync(Guid characterId)
    {
        var character = await _db.Characters
            .Include(c => c.StoryProfile)
            .FirstOrDefaultAsync(c => c.Id == characterId);

        if (character is null)
            return ApiResponseDto<CharacterStoryProfileResponseDto>.NotFound($"Character with ID {characterId} not found.");

        var error = await ValidateProjectAccessAsync<CharacterStoryProfileResponseDto>(character.MetaInfo.ProjectId);
        if (error != null) return error;

        if (character.StoryProfile is null)
            return ApiResponseDto<CharacterStoryProfileResponseDto>.NotFound($"Story profile for character {characterId} not found.");

        var profile = character.StoryProfile;

        return ApiResponseDto<CharacterStoryProfileResponseDto>.Success(new CharacterStoryProfileResponseDto
        {
            Id = profile.Id,
            CharacterId = profile.CharacterId,
            OriginStory = profile.OriginStory,
            FamilyBackground = profile.FamilyBackground,
            Backstory = profile.Backstory,
            PersonalityTraits = profile.PersonalityTraits,
            Motivation = profile.Motivation,
            Fear = profile.Fear,
            Beliefs = profile.Beliefs,
            SpeechPattern = profile.SpeechPattern,
            Quirks = profile.Quirks,
            StoryRole = profile.StoryRole,
            ArcType = profile.ArcType,
            ArcSummary = profile.ArcSummary,
            KeyRelationships = profile.KeyRelationships
        });
    }

    // ========================================================================
    // POST - Create a new story profile for a character
    // ========================================================================

    public async Task<ApiResponseDto<CharacterStoryProfileResponseDto>> CreateStoryProfileAsync(Guid projectId, Guid characterId, CharacterStoryProfileCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CharacterStoryProfileResponseDto>(projectId);
        if (error != null) return error;

        // Verify character exists and belongs to this project
        var character = await _db.Characters
            .Include(c => c.StoryProfile)
            .FirstOrDefaultAsync(c => c.Id == characterId);

        if (character is null)
            return ApiResponseDto<CharacterStoryProfileResponseDto>.NotFound($"Character with ID {characterId} not found.");

        if (character.MetaInfo.ProjectId != projectId)
            return ApiResponseDto<CharacterStoryProfileResponseDto>.BadRequest("Character does not belong to this project.");

        // Check if story profile already exists (one-to-one relationship)
        if (character.StoryProfile != null)
            return ApiResponseDto<CharacterStoryProfileResponseDto>.Conflict($"Story profile already exists for character {characterId}.");

        var profile = new CharacterStoryProfile
        {
            Id = Guid.NewGuid(),
            CharacterId = characterId,
            OriginStory = createDto.OriginStory,
            FamilyBackground = createDto.FamilyBackground,
            Backstory = createDto.Backstory,
            PersonalityTraits = createDto.PersonalityTraits,
            Motivation = createDto.Motivation,
            Fear = createDto.Fear,
            Beliefs = createDto.Beliefs,
            SpeechPattern = createDto.SpeechPattern,
            Quirks = createDto.Quirks,
            StoryRole = createDto.StoryRole,
            ArcType = createDto.ArcType,
            ArcSummary = createDto.ArcSummary,
            KeyRelationships = createDto.KeyRelationships
        };

        _db.CharacterStoryProfiles.Add(profile);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CharacterStoryProfileResponseDto>.Success(new CharacterStoryProfileResponseDto
        {
            Id = profile.Id,
            CharacterId = profile.CharacterId,
            OriginStory = profile.OriginStory,
            FamilyBackground = profile.FamilyBackground,
            Backstory = profile.Backstory,
            PersonalityTraits = profile.PersonalityTraits,
            Motivation = profile.Motivation,
            Fear = profile.Fear,
            Beliefs = profile.Beliefs,
            SpeechPattern = profile.SpeechPattern,
            Quirks = profile.Quirks,
            StoryRole = profile.StoryRole,
            ArcType = profile.ArcType,
            ArcSummary = profile.ArcSummary,
            KeyRelationships = profile.KeyRelationships
        });
    }

    // ========================================================================
    // PUT - Update story profile
    // ========================================================================

    public async Task<ApiResponseDto<CharacterStoryProfileResponseDto>> UpdateStoryProfileAsync(Guid characterId, CharacterStoryProfileUpdateDto updateDto)
    {
        var character = await _db.Characters
            .Include(c => c.StoryProfile)
            .FirstOrDefaultAsync(c => c.Id == characterId);

        if (character is null)
            return ApiResponseDto<CharacterStoryProfileResponseDto>.NotFound($"Character with ID {characterId} not found.");

        var profile = character.StoryProfile;
        if (profile is null)
            return ApiResponseDto<CharacterStoryProfileResponseDto>.NotFound($"Story profile for character {characterId} not found.");

        var error = await ValidateProjectAccessAsync<CharacterStoryProfileResponseDto>(character.MetaInfo.ProjectId);
        if (error != null) return error;

        // Apply updates (only non-null values are updated)
        if (updateDto.OriginStory != null) profile.OriginStory = updateDto.OriginStory;
        if (updateDto.FamilyBackground != null) profile.FamilyBackground = updateDto.FamilyBackground;
        if (updateDto.Backstory != null) profile.Backstory = updateDto.Backstory;
        if (updateDto.PersonalityTraits != null) profile.PersonalityTraits = updateDto.PersonalityTraits;
        if (updateDto.Motivation != null) profile.Motivation = updateDto.Motivation;
        if (updateDto.Fear != null) profile.Fear = updateDto.Fear;
        if (updateDto.Beliefs != null) profile.Beliefs = updateDto.Beliefs;
        if (updateDto.SpeechPattern != null) profile.SpeechPattern = updateDto.SpeechPattern;
        if (updateDto.Quirks != null) profile.Quirks = updateDto.Quirks;
        if (updateDto.StoryRole.HasValue) profile.StoryRole = updateDto.StoryRole.Value;
        if (updateDto.ArcType != null) profile.ArcType = updateDto.ArcType;
        if (updateDto.ArcSummary != null) profile.ArcSummary = updateDto.ArcSummary;
        if (updateDto.KeyRelationships != null) profile.KeyRelationships = updateDto.KeyRelationships;

        await _db.SaveChangesAsync();

        return ApiResponseDto<CharacterStoryProfileResponseDto>.Success(new CharacterStoryProfileResponseDto
        {
            Id = profile.Id,
            CharacterId = profile.CharacterId,
            OriginStory = profile.OriginStory,
            FamilyBackground = profile.FamilyBackground,
            Backstory = profile.Backstory,
            PersonalityTraits = profile.PersonalityTraits,
            Motivation = profile.Motivation,
            Fear = profile.Fear,
            Beliefs = profile.Beliefs,
            SpeechPattern = profile.SpeechPattern,
            Quirks = profile.Quirks,
            StoryRole = profile.StoryRole,
            ArcType = profile.ArcType,
            ArcSummary = profile.ArcSummary,
            KeyRelationships = profile.KeyRelationships
        });
    }

    // ========================================================================
    // DELETE - Delete story profile
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteStoryProfileAsync(Guid characterId)
    {
        var character = await _db.Characters
            .Include(c => c.StoryProfile)
            .FirstOrDefaultAsync(c => c.Id == characterId);

        if (character is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Character with ID {characterId} not found.");

        var profile = character.StoryProfile;
        if (profile is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Story profile for character {characterId} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(character.MetaInfo.ProjectId);
        if (error != null) return error;

        _db.CharacterStoryProfiles.Remove(profile);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = profile.Id,
            ProjectId = character.MetaInfo.ProjectId
        });
    }
}
