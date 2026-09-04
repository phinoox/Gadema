using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Projects;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Models.Projects;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gadema.Api.Services.Projects;

/// <summary>
/// Service for managing Project Tags and their assignments to Projects.
/// </summary>
public class ProjectTagService : IProjectTagService
{
    private readonly GameDbContext _context;
    private readonly ILogger<ProjectTagService> _logger;

    public ServiceTypeEnum ServiceType => ServiceTypeEnum.ProjectTagService;

    public ProjectTagService(GameDbContext context, ILogger<ProjectTagService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all tags associated with a specific project.
    /// </summary>
    public async Task<ApiResponseDto<List<ProjectTagDto>>> GetProjectTagsAsync(Guid projectId)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null)
            return ApiResponseDto<List<ProjectTagDto>>.NotFound("Project not found.");

        // Fetch tags via the junction table
        var assignments = await _context.ProjectTagRelations
            .Where(apt => apt.ProjectId == projectId)
            .Include(apt => apt.ProjectTag)
            .ToListAsync();

        var tags = assignments.Select(apt => new ProjectTagDto
        {
            Id = apt.ProjectTag.Id,
            Name = apt.ProjectTag.Name,
            Slug = apt.ProjectTag.Slug,
            ColorHex = apt.ProjectTag.ColorHex,
            UsageCount = _context.ProjectTagRelations.Count(a => a.ProjectTagId == apt.ProjectTag.Id)
        }).ToList();

        return ApiResponseDto<List<ProjectTagDto>>.Success(tags);
    }

    /// <summary>
    /// Retrieves all available project tags (for autocomplete).
    /// </summary>
    public async Task<ApiResponseDto<List<ProjectTagDto>>> GetAllProjectTagsAsync()
    {
        var tags = await _context.ProjectTags
            .OrderBy(t => t.Name)
            .Select(t => new ProjectTagDto
            {
                Id = t.Id,
                Name = t.Name,
                Slug = t.Slug,
                ColorHex = t.ColorHex
            })
            .ToListAsync();

        return ApiResponseDto<List<ProjectTagDto>>.Success(tags);
    }

    /// <summary>
    /// Adds tags to a project.
    /// </summary>
    public async Task<ApiResponseDto<ProjectTagDto>> AddTagsToProjectAsync(Guid projectId, AddProjectTagsDto dto)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null)
            return ApiResponseDto<ProjectTagDto>.NotFound("Project not found.");

        var addedTags = new List<ProjectTagDto>();

        foreach (var tagId in dto.TagIds)
        {
            var tag = await _context.ProjectTags.FindAsync(tagId);
            if (tag == null)
            {
                _logger.LogWarning("Attempted to add non-existent ProjectTag {TagId} to Project {ProjectId}", tagId, projectId);
                continue;
            }

            // Check for duplicates
            var exists = await _context.ProjectTagRelations
                .AnyAsync(a => a.ProjectId == projectId && a.ProjectTagId == tagId);

            if (!exists)
            {
                var assignment = new ProjectTagRelation
                {
                    ProjectId = projectId,
                    ProjectTagId = tagId
                };

                _context.ProjectTagRelations.Add(assignment);
                addedTags.Add(new ProjectTagDto { Id = tag.Id, Name = tag.Name });
            }
        }

        await _context.SaveChangesAsync();

        return ApiResponseDto<ProjectTagDto>.Success(addedTags.FirstOrDefault());
    }

    /// <summary>
    /// Removes tags from a project.
    /// </summary>
    public async Task<ApiResponseDto<ProjectTagDto>> RemoveTagsFromProjectAsync(Guid projectId, RemoveProjectTagsDto dto)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null)
            return ApiResponseDto<ProjectTagDto>.NotFound("Project not found.");

        var removedCount = 0;

            var assignment = await _context.ProjectTagRelations
                .FirstOrDefaultAsync(a => a.ProjectId == projectId && a.ProjectTagId == dto.TagId);

            if (assignment != null)
            {
                _context.ProjectTagRelations.Remove(assignment);
                removedCount++;
            }
        

        if (removedCount > 0)
            await _context.SaveChangesAsync();

        return ApiResponseDto<ProjectTagDto>.Success(null);
    }

    /// <summary>
    /// Creates a new global Project Tag.
    /// </summary>
    public async Task<ApiResponseDto<ProjectTagDto>> CreateProjectTagAsync(CreateProjectTagDto dto)
    {
        // Check for duplicate names
        var exists = await _context.ProjectTags.AnyAsync(t => t.Name.ToLower() == dto.Name.ToLower());
        if (exists)
            return ApiResponseDto<ProjectTagDto>.Conflict("Tag name already exists.");

        var tag = new ProjectTag
        {
            Name = dto.Name,
            Slug = dto.Slug ?? dto.Name.ToLower().Replace(" ", "-"),
            ColorHex = dto.ColorHex
        };

        _context.ProjectTags.Add(tag);
        await _context.SaveChangesAsync();

        return ApiResponseDto<ProjectTagDto>.Success(new ProjectTagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Slug = tag.Slug,
            ColorHex = tag.ColorHex
        });
    }

    /// <summary>
    /// Deletes a global Project Tag.
    /// </summary>
    public async Task<ApiResponseDto<SimpleResponseDto>> DeleteProjectTagAsync(RemoveProjectTagsDto removeProjectTagsDto)
    {
        var tagId = removeProjectTagsDto.TagId;
            var tag = await _context.ProjectTags.FindAsync(tagId);
            if (tag == null)
                return ApiResponseDto<SimpleResponseDto>.NotFound("Tag not found.");

            // Check if tag is in use
            var isUsed = await _context.ProjectTagRelations.AnyAsync(a => a.ProjectTagId == tagId);
            if (isUsed)
                return ApiResponseDto<SimpleResponseDto>.Conflict("Cannot delete tag that is currently assigned to a project.");

            _context.ProjectTags.Remove(tag);
       
        await _context.SaveChangesAsync();

        return ApiResponseDto<SimpleResponseDto>.Success(new SimpleResponseDto(){Success= true, Message = $"successfully removed tag {tagId}"});
    }
}