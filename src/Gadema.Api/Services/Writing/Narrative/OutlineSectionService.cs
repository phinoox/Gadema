// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Writing.Narrative;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Writing.Narrative;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.Narrative;

/// <summary>
/// Service for managing OutlineSections within the narrative domain.
/// Handles CRUD operations including authorization.
/// </summary>
public class OutlineSectionService : CoreService
{
    public OutlineSectionService(GameDbContext db, ILogger<OutlineSectionService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List all outline sections for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<OutlineSectionResponseDto>>> GetOutlineSectionsAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<OutlineSectionResponseDto>>(projectId);
        if (error != null) return error;

        var sections = await _db.OutlineSections
            .Include(os => os.StoryOutline)
            .ThenInclude(so => so.ContentMetaInfo)
            .Where(os => os.StoryOutline.ContentMetaInfo.ProjectId == projectId)
            .OrderBy(os => os.StoryOutlineId)
            .ThenBy(os => os.SortOrder)
            .Select(os => new OutlineSectionResponseDto
            {
                Id = os.Id,
                StoryOutlineId = os.StoryOutlineId,
                Title = os.Title,
                RawText = os.RawText,
                SortOrder = os.SortOrder,
                LinkedBeatIds = os.LinkedBeats.Select(lb => lb.Id).ToList() ?? new List<Guid>()
            })
            .ToListAsync();

        return ApiResponseDto<IEnumerable<OutlineSectionResponseDto>>.Success(sections);
    }

    // ========================================================================
    // GET - Single outline section by ID
    // ========================================================================

    public async Task<ApiResponseDto<OutlineSectionResponseDto>> GetOutlineSectionAsync(Guid id)
    {
        var section = await _db.OutlineSections
            .Include(os => os.StoryOutline)
            .ThenInclude(so => so.ContentMetaInfo)
            .Include(os => os.LinkedBeats)
            .FirstOrDefaultAsync(os => os.Id == id);

        if (section is null)
            return ApiResponseDto<OutlineSectionResponseDto>.NotFound($"Outline section with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<OutlineSectionResponseDto>(section.StoryOutline.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<OutlineSectionResponseDto>.Success(new OutlineSectionResponseDto
        {
            Id = section.Id,
            StoryOutlineId = section.StoryOutlineId,
            Title = section.Title,
            RawText = section.RawText,
            SortOrder = section.SortOrder,
            LinkedBeatIds = section.LinkedBeats?.Select(lb => lb.Id).ToList() ?? new List<Guid>()
        });
    }

    // ========================================================================
    // POST - Create a new outline section
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateOutlineSectionAsync(Guid projectId, OutlineSectionCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        // Validate that the StoryOutline belongs to the project
        var outline = await _db.StoryOutlines
            .Include(so => so.ContentMetaInfo)
            .FirstOrDefaultAsync(so => so.Id == createDto.StoryOutlineId);

        if (outline is null)
            return ApiResponseDto<CreateResponseDto>.NotFound($"StoryOutline with ID {createDto.StoryOutlineId} not found.");

        if (outline.ContentMetaInfo.ProjectId != projectId)
            return ApiResponseDto<CreateResponseDto>.BadRequest("StoryOutline does not belong to the specified project.");

        var section = new OutlineSection
        {
            Id = Guid.NewGuid(),
            StoryOutlineId = createDto.StoryOutlineId,
            Title = createDto.Title,
            RawText = createDto.RawText,
            SortOrder = createDto.SortOrder ?? 0,
        };

        _db.OutlineSections.Add(section);
        await _db.SaveChangesAsync();

        // Link beats if provided
        if (createDto.LinkedBeatIds != null && createDto.LinkedBeatIds.Any())
        {
            var beats = await _db.StoryBeats
                .Where(sb => createDto.LinkedBeatIds.Contains(sb.Id))
                .ToListAsync();

            section.LinkedBeats = beats;
            await _db.SaveChangesAsync();
        }

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = section.Id,
            MetaInfoId = section.Id, // OutlineSection doesn't have its own ContentMetaInfo
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of an outline section
    // ========================================================================

    public async Task<ApiResponseDto<OutlineSectionResponseDto>> UpdateOutlineSectionAsync(Guid id, OutlineSectionUpdateDto updateDto)
    {
        var section = await _db.OutlineSections
            .Include(os => os.StoryOutline)
            .ThenInclude(so => so.ContentMetaInfo)
            .Include(os => os.LinkedBeats)
            .FirstOrDefaultAsync(os => os.Id == id);

        if (section is null)
            return ApiResponseDto<OutlineSectionResponseDto>.NotFound($"Outline section with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<OutlineSectionResponseDto>(section.StoryOutline.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        if (updateDto.Title != null)
            section.Title = updateDto.Title;

        if (updateDto.RawText != null)
            section.RawText = updateDto.RawText;

        if (updateDto.SortOrder.HasValue)
            section.SortOrder = updateDto.SortOrder.Value;

        if (updateDto.StoryOutlineId != Guid.Empty)
            section.StoryOutlineId = updateDto.StoryOutlineId;

        // Update linked beats if provided
        if (updateDto.LinkedBeatIds != null)
        {
            var beats = await _db.StoryBeats
                .Where(sb => updateDto.LinkedBeatIds.Contains(sb.Id))
                .ToListAsync();

            section.LinkedBeats = beats;
        }

        await _db.SaveChangesAsync();

        return ApiResponseDto<OutlineSectionResponseDto>.Success(new OutlineSectionResponseDto
        {
            Id = section.Id,
            StoryOutlineId = section.StoryOutlineId,
            Title = section.Title,
            RawText = section.RawText,
            SortOrder = section.SortOrder,
            LinkedBeatIds = section.LinkedBeats?.Select(lb => lb.Id).ToList() ?? new List<Guid>()
        });
    }

    // ========================================================================
    // DELETE - Remove an outline section
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteOutlineSectionAsync(Guid id)
    {
        var section = await _db.OutlineSections
            .Include(os => os.StoryOutline)
            .ThenInclude(so => so.ContentMetaInfo)
            .FirstOrDefaultAsync(os => os.Id == id);

        if (section is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Outline section with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(section.StoryOutline.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        _db.OutlineSections.Remove(section);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = section.StoryOutline.ContentMetaInfo.ProjectId
        });
    }
}
