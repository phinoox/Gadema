// =============================================================================
using Gadema.Api.Services.Search;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Search;
using Gadema.Core.Dtos.Writing.Characters;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Writing.Characters;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.Characters;

/// <summary>
/// Service for managing Characters - the central glue entity in the character domain.
/// Supports progressive creation: Identity (ContentMetaInfo + Character) first, then modular components.
/// </summary>
[ServiceLifetime(ServiceLifetime.Scoped)] public class CharacterService : CoreService, ISearchableProvider
{
    public CharacterService(GameDbContext db, ILogger<CharacterService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }


    /// <summary>
    /// Implements ISearchableProvider. Provides matches for the SearchOrchestrator.
    /// </summary>
    public async Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId)
    {
        var dbQuery = _db.Characters
            .Include(c => c.ContentMetaInfo)
            .AsQueryable();

        // 1. Filter by Project Scope if provided
        if (projectId.HasValue)
        {
            dbQuery = dbQuery.Where(c => c.ContentMetaInfo.ProjectId == projectId.Value);
        }

        // 2. Search across identity properties (Title in ContentMetaInfo, Name/Nickname in Character)
        var matches = await dbQuery
            .Where(c => c.ContentMetaInfo.Title.Contains(query) || 
                        c.Name.Contains(query) || 
                        (c.NickName != null && c.NickName.Contains(query)))
            .ToListAsync();

        // 3. Project into the standardized SearchHitDto
        return matches.Select(c => new SearchHitDto
        {
            ResourceId = c.Id,
            DisplayName = c.ContentMetaInfo.Title, // Using Title as the primary display name per convention
            Slug = c.ContentMetaInfo.Slug,
            ResourceType = "Character",
            ScopeId = c.ContentMetaInfo.ProjectId,
            ResourceLink = $"/api/v1/projects/{c.ContentMetaInfo.ProjectId}/characters/{c.Id}"
        });
    }


    // ========================================================================
    // GET - List all characters for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<CharacterResponseDto>>> GetCharactersAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<CharacterResponseDto>>(projectId);
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

    // ========================================================================
    // GET - Single character by ID
    // ========================================================================

    public async Task<ApiResponseDto<CharacterResponseDto>> GetCharacterAsync(Guid id)
    {
        var character = await _db.Characters
            .Include(c => c.ContentMetaInfo)
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Faction)
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Location)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (character is null)
            return ApiResponseDto<CharacterResponseDto>.NotFound($"Character with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<CharacterResponseDto>(character.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<CharacterResponseDto>.Success(CreateResponseDto(character));
    }

    // ========================================================================
    // POST - Create the core identity (Progressive Step 1)
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateCharacterAsync(Guid projectId, CharacterCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        // 1. Create ContentMetaInfo for the character's identity (Title/Name lives here)
        var ContentMetaInfo = CreateMetaInfo(projectId, ContentTypeEnum.Character, createDto.CreateData);
        _db.MetaInfos.Add(ContentMetaInfo);
        await _db.SaveChangesAsync();

        // 2. Create the Character glue entity
        var character = new Character
        {
            Id = Guid.NewGuid(),
            MetaInfoId = ContentMetaInfo.Id,
            Name = createDto.Name, // Fallback for non-meta identity fields
            NickName = createDto.NickName,
            CurrentStateId = null
        };

        _db.Characters.Add(character);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = character.Id,
            MetaInfoId = ContentMetaInfo.Id,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of identity and linkage
    // ========================================================================

    public async Task<ApiResponseDto<CharacterResponseDto>> UpdateCharacterAsync(Guid id, CharacterUpdateDto updateDto)
    {
        var character = await _db.Characters
            .Include(c => c.ContentMetaInfo)
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Faction)
            .Include(c => c.CurrentState).ThenInclude(cs => cs!.Location)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (character is null)
            return ApiResponseDto<CharacterResponseDto>.NotFound($"Character with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<CharacterResponseDto>(character.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        // 1. Update ContentMetaInfo (Name/Title/Status lives here per convention)
        if (updateDto.ContentMetaInfo != null)
        {
            ApplyMetaInfoUpdates(character.ContentMetaInfo, updateDto.ContentMetaInfo);
        }

        // 2. Update Character Glue properties
        if (updateDto.NickName != null) character.NickName = updateDto.NickName;

        // 3. Update CurrentState linkage if provided
        if (updateDto.CurrentStateId.HasValue)
        {
            var newState = await _db.CharacterStates
                .Include(cs => cs.ContentMetaInfo)
                .FirstOrDefaultAsync(cs => cs.Id == updateDto.CurrentStateId.Value);

            if (newState is null)
                return ApiResponseDto<CharacterResponseDto>.NotFound($"CharacterState with ID {updateDto.CurrentStateId.Value} not found.");

            // Security check: Ensure the state belongs to the same project
            if (newState.ContentMetaInfo.ProjectId != character.ContentMetaInfo.ProjectId)
                return ApiResponseDto<CharacterResponseDto>.BadRequest("The target CharacterState does not belong to this project.");

            character.CurrentStateId = updateDto.CurrentStateId.Value;
        }

        await _db.SaveChangesAsync();

        return ApiResponseDto<CharacterResponseDto>.Success(CreateResponseDto(character));
    }

    // ========================================================================
    // DELETE - Remove a character (and its ContentMetaInfo)
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteCharacterAsync(Guid id)
    {
        var character = await _db.Characters
            .Include(c => c.ContentMetaInfo)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (character is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Character with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(character.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        // Deleting ContentMetaInfo will cascade to the Character record via DeleteBehavior.Cascade/Restrict 
        // and remove the character record itself.
        _db.MetaInfos.Remove(character.ContentMetaInfo);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = character.ContentMetaInfo.ProjectId
        });
    }

    private CharacterResponseDto CreateResponseDto(Character character)
    {
        return new CharacterResponseDto()
        {
            Id = character.Id,
            MetaInfoId = character.MetaInfoId,
            // Map ContentMetaInfo Title to Response Name per convention
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