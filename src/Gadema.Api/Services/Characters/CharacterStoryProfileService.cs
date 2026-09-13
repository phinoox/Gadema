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
/// Service for managing CharacterStoryProfile - the static backstory and personality of a character.
/// Access is validated via the parent Character's MetaInfo.
/// </summary>
public class CharacterStoryProfileService : CoreService
{
    public CharacterStoryProfileService(GameDbContext db, ILogger<CharacterStoryProfileService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    /// <summary>Creates a response DTO from a CharacterStoryProfile entity.</summary>
    private CharacterStoryProfileResponseDto CreateResponseDto(CharacterStoryProfile profile)
        => new()
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
        };

    // ========================================================================
    // GET - List all profiles for a project (via Characters)
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<CharacterStoryProfileResponseDto>>> GetProfilesAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<CharacterStoryProfileResponseDto>>(projectId);
        if (error != null) return error;

        var profiles = await _db.CharacterStoryProfiles
            .Include(p => p.Character)
                .ThenInclude(c => c.MetaInfo)
            .Where(p => p.Character.MetaInfo.ProjectId == projectId)
            .ToListAsync();
        
        var projectedProfiles = profiles.Select(CreateResponseDto).ToList();

        return ApiResponseDto<IEnumerable<CharacterStoryProfileResponseDto>>.Success(projectedProfiles);
    }

    // ========================================================================
    // GET - Single profile by ID
    // ========================================================================

    public async Task<ApiResponseDto<CharacterStoryProfileResponseDto>> GetProfileAsync(Guid id)
    {
        var profile = await _db.CharacterStoryProfiles
            .Include(p => p.Character)
                .ThenInclude(c => c.MetaInfo)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (profile is null)
            return ApiResponseDto<CharacterStoryProfileResponseDto>.NotFound($"Story profile with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<CharacterStoryProfileResponseDto>(profile.Character.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<CharacterStoryProfileResponseDto>.Success(CreateResponseDto(profile));
    }

    // ========================================================================
    // POST - Create a profile for an existing character
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateProfileAsync(Guid projectId,Guid characterId, CharacterStoryProfileCreateDto createDto)
    {
        // 1. Validate character exists and belongs to project
        var character = await _db.Characters
            .Include(c => c.MetaInfo)
            .FirstOrDefaultAsync(c => c.Id == characterId && c.MetaInfo.ProjectId == projectId);

        if (character is null)
            return ApiResponseDto<CreateResponseDto>.NotFound("The specified character does not exist in this project.");

        // 2. Create the profile
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

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = profile.Id,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Update a profile
    // ========================================================================

    public async Task<ApiResponseDto<CharacterStoryProfileResponseDto>> UpdateProfileAsync(Guid id, CharacterStoryProfileUpdateDto updateDto)
    {
        var profile = await _db.CharacterStoryProfiles
            .Include(p => p.Character)
                .ThenInclude(c => c.MetaInfo)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (profile is null)
            return ApiResponseDto<CharacterStoryProfileResponseDto>.NotFound($"Story profile with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<CharacterStoryProfileUpdateDto>(profile.Character.MetaInfo.ProjectId);
        if (error != null) return ApiResponseDto<CharacterStoryProfileResponseDto>.Unauthorized(error.Message ?? "not authorized");

        // Update fields if provided in DTO
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

        return ApiResponseDto<CharacterStoryProfileResponseDto>.Success(CreateResponseDto(profile));
    }

    // ========================================================================
    // DELETE - Remove a profile
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteProfileAsync(Guid id)
    {
        var profile = await _db.CharacterStoryProfiles
            .Include(p => p.Character)
                .ThenInclude(c => c.MetaInfo)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (profile is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Story profile with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(profile.Character.MetaInfo.ProjectId);
        if (error != null) return error;

        _db.CharacterStoryProfiles.Remove(profile);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = profile.Character.MetaInfo.ProjectId
        });
    }
}