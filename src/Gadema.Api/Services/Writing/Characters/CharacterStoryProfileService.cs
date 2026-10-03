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

[ServiceLifetime(ServiceLifetime.Scoped)] public class CharacterStoryProfileService : DomainService
{
    private WritingDbContext _db;

    public CharacterStoryProfileService( WritingDbContext db,
        ILogger<CharacterStoryProfileService> logger,  
        ICoreServicesProvider coreServices) 
        : base(coreServices,logger) { _db = db; }

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

    public async Task<ApiResponseDto<IEnumerable<CharacterStoryProfileResponseDto>>> GetProfilesAsync(Guid projectId)
    {
        var error = await CheckAccessAsync<IEnumerable<CharacterStoryProfileResponseDto>>(projectId, Permission.CanView);
        if (error != null) return error;

        var profiles = await _db.CharacterStoryProfiles
            .Include(p => p.Character)
                .ThenInclude(c => c.ContentMetaInfo)
            .Where(p => p.Character.ContentMetaInfo.ProjectId == projectId)
            .ToListAsync();
        
        return ApiResponseDto<IEnumerable<CharacterStoryProfileResponseDto>>.Success(profiles.Select(CreateResponseDto));
    }

    public async Task<ApiResponseDto<CharacterStoryProfileResponseDto>> GetProfileAsync(Guid id)
    {
        var profile = await _db.CharacterStoryProfiles
            .Include(p => p.Character)
                .ThenInclude(c => c.ContentMetaInfo)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (profile is null)
            return ApiResponseDto<CharacterStoryProfileResponseDto>.NotFound($"Story profile with ID {id} not found.");

        var error = await CheckAccessAsync<CharacterStoryProfileResponseDto>(profile.Character.ContentMetaInfo.ProjectId, Permission.CanView);
        if (error != null) return error;

        return ApiResponseDto<CharacterStoryProfileResponseDto>.Success(CreateResponseDto(profile));
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateProfileAsync(Guid projectId, Guid characterId, CharacterStoryProfileCreateDto createDto)
    {
        var error = await CheckAccessAsync<CreateResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        var character = await _db.Characters
            .Include(c => c.ContentMetaInfo)
            .FirstOrDefaultAsync(c => c.Id == characterId && c.ContentMetaInfo.ProjectId == projectId);

        if (character is null)
            return ApiResponseDto<CreateResponseDto>.NotFound("The specified character does not exist in this project.");

        var profile = new CharacterStoryProfile
        {
            Id = characterId, // Law I: Unification - Profile ID matches Character ID
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

    public async Task<ApiResponseDto<CharacterStoryProfileResponseDto>> UpdateProfileAsync(Guid id, CharacterStoryProfileUpdateDto updateDto)
    {
        var profile = await _db.CharacterStoryProfiles
            .Include(p => p.Character)
                .ThenInclude(c => c.ContentMetaInfo)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (profile is null)
            return ApiResponseDto<CharacterStoryProfileResponseDto>.NotFound($"Story profile with ID {id} not found.");

        var error = await CheckAccessAsync<CharacterStoryProfileResponseDto>(profile.Character.ContentMetaInfo.ProjectId, Permission.CanEdit);
        if (error != null) return error;

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

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteProfileAsync(Guid id)
    {
        var profile = await _db.CharacterStoryProfiles
            .Include(p => p.Character)
                .ThenInclude(c => c.ContentMetaInfo)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (profile is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Story profile with ID {id} not found.");

        var error = await CheckAccessAsync<DeleteResponseDto>(profile.Character.ContentMetaInfo.ProjectId, Permission.CanDelete);
        if (error != null) return error;

        _db.CharacterStoryProfiles.Remove(profile);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = profile.Character.ContentMetaInfo.ProjectId.Value
        });
    }
}