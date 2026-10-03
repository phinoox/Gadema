using Gadema.Api.CoreServices;
using Gadema.Api.CoreServices.Strategies;
using Gadema.Api.Services.Search;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Search;
using Gadema.Core.Dtos.Writing.Characters;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Core.Models.Writing.Characters;
using Gadema.Data.Database.Core;
using Gadema.Data.Database.Writing;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.Characters;

[ServiceLifetime(ServiceLifetime.Scoped)] public class CharacterService : DomainService, ISearchableProvider
{
    private WritingDbContext _db;

    public CharacterService( WritingDbContext db,
        ILogger<CharacterService> logger,  
        ICoreServicesProvider coreServices) 
        : base(coreServices,logger) { _db = db; }


    public async Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId)
    {
        var dbQuery = _db.Characters
            .Include(c => c.ContentMetaInfo)
            .AsQueryable();

        if (projectId.HasValue)
        {
            dbQuery = dbQuery.Where(c => c.ContentMetaInfo.ProjectId == projectId.Value);
        }

        var matches = await dbQuery
            .Where(c => c.ContentMetaInfo.Title.Contains(query) || 
                        c.Name.Contains(query) || 
                        (c.NickName != null && c.NickName.Contains(query)))
            .ToListAsync();

        return matches.Select(c => new SearchHitDto
        {
            ResourceId = c.Id,
            DisplayName = c.ContentMetaInfo.Title,
            Slug = c.ContentMetaInfo.Slug,
            ResourceType = "Character",
            ScopeId = c.ContentMetaInfo.ProjectId,
            ResourceLink = $"/api/v1/projects/{c.ContentMetaInfo.ProjectId}/characters/{c.Id}"
        });
    }

    public async Task<ApiResponseDto<IEnumerable<CharacterResponseDto>>> GetCharactersAsync(Guid projectId)
    {
        var error = await CheckAccessAsync<IEnumerable<CharacterResponseDto>>(projectId, Permission.CanView);
        if (error != null) return error;

        var characters = await _db.Characters
            .Include(c => c.ContentMetaInfo)
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Faction)
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Location)
            .Where(c => c.ContentMetaInfo.ProjectId == projectId)
            .OrderBy(c => c.Name)
            .ToListAsync();

        var projected = characters.Select(CreateResponseDto).ToList();
        return ApiResponseDto<IEnumerable<CharacterResponseDto>>.Success(projected);
    }

    public async Task<ApiResponseDto<CharacterResponseDto>> GetCharacterAsync(Guid id)
    {
        var character = await _db.Characters
            .Include(c => c.ContentMetaInfo)
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Faction)
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Location)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (character is null)
            return ApiResponseDto<CharacterResponseDto>.NotFound($"Character with ID {id} not found.");

        var error = await CheckAccessAsync<CharacterResponseDto>(character.ContentMetaInfo.ProjectId, Permission.CanView);
        if (error != null) return error;

        return ApiResponseDto<CharacterResponseDto>.Success(CreateResponseDto(character));
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateCharacterAsync(Guid projectId, CharacterCreateDto createDto)
    {
        var error = await CheckAccessAsync<CreateResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        var contentMetaInfo = await _core.MetadataService.CreateAsync<ContentMetaInfo>(createDto.CreateData, m =>
        {
            m.ProjectId = projectId;
            m.ContentType = ContentTypeEnum.Character;
        });

        // Apply Law I: Body.Id must equal Soul.Id (contentMetaInfo.Id)
        var character = new Character
        {
            Id = contentMetaInfo.Id, // Unification!
            MetaInfoId = contentMetaInfo.Id,
            Name = createDto.Name, 
            NickName = createDto.NickName,
            CurrentStateId = null
        };

        _db.Characters.Add(character);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = character.Id,
            MetaInfoId = contentMetaInfo.Id,
            ProjectId = projectId
        });
    }

    public async Task<ApiResponseDto<CharacterResponseDto>> UpdateCharacterAsync(Guid id, CharacterUpdateDto updateDto)
    {
        var character = await _db.Characters
            .Include(c => c.ContentMetaInfo)
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Faction)
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Location)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (character is null)
            return ApiResponseDto<CharacterResponseDto>.NotFound($"Character with ID {id} not found.");

        var error = await CheckAccessAsync<CharacterResponseDto>(character.ContentMetaInfo.ProjectId, Permission.CanEdit);
        if (error != null) return error;

        if (updateDto.ContentMetaInfo != null)
        {
            var success = await SyncIdentityAsync<ContentIdentityStrategy>(character.MetaInfoId, updateDto.ContentMetaInfo);
            if (!success) return ApiResponseDto<CharacterResponseDto>.ServerError("Sync failed.");
        }

        if (updateDto.NickName != null) character.NickName = updateDto.NickName;

        if (updateDto.CurrentStateId.HasValue)
        {
            var newState = await _db.CharacterStates
                .Include(cs => cs.ContentMetaInfo)
                .FirstOrDefaultAsync(cs => cs.Id == updateDto.CurrentStateId.Value);

            if (newState is null)
                return ApiResponseDto<CharacterResponseDto>.NotFound($"CharacterState with ID {updateDto.CurrentStateId.Value} not found.");

            if (newState.ContentMetaInfo.ProjectId != character.ContentMetaInfo.ProjectId)
                return ApiResponseDto<CharacterResponseDto>.BadRequest("The target CharacterState does not belong to this project.");

            character.CurrentStateId = updateDto.CurrentStateId.Value;
        }

        await _db.SaveChangesAsync();

        return ApiResponseDto<CharacterResponseDto>.Success(CreateResponseDto(character));
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteCharacterAsync(Guid id)
    {
        var character = await _db.Characters
            .Include(c => c.ContentMetaInfo)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (character is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Character with ID {id} not found.");

        var error = await CheckAccessAsync<DeleteResponseDto>(character.ContentMetaInfo.ProjectId, Permission.CanDelete);
        if (error != null) return error;

        _db.Set<ContentMetaInfo>().Remove(character.ContentMetaInfo);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = character.ContentMetaInfo.ProjectId.Value
        });
    }

    private CharacterResponseDto CreateResponseDto(Character character)
    {
        return new CharacterResponseDto()
        {
            Id = character.Id,
            MetaInfoId = character.MetaInfoId,
            Name = character.ContentMetaInfo.Title, 
            NickName = character.NickName,
            Status = character.ContentMetaInfo.Status,
            IsPublic = character.ContentMetaInfo.IsPublic,
            CreatedAt = character.ContentMetaInfo.CreatedAt,
            LastModifiedAt = character.ContentMetaInfo.LastModifiedAt,
            CurrentStateId = character.CurrentStateId,
            StoryProfileId = character.StoryProfileId
        };
    }
}