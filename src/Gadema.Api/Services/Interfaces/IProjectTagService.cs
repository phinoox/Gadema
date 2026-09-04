using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Projects;
using Gadema.Core.Dtos.Response;

namespace Gadema.Core.Services;

public interface IProjectTagService : IGademaService
{
    /// <summary>
    /// Retrieves all tags associated with a specific project.
    /// </summary>
    Task<ApiResponseDto<List<ProjectTagDto>>> GetProjectTagsAsync(Guid projectId);

    /// <summary>
    /// Retrieves all available project tags (for autocomplete).
    /// </summary>
    Task<ApiResponseDto<List<ProjectTagDto>>> GetAllProjectTagsAsync();

    /// <summary>
    /// Adds tags to a project.
    /// </summary>
    Task<ApiResponseDto<ProjectTagDto>> AddTagsToProjectAsync(Guid projectId, AddProjectTagsDto dto);

    /// <summary>
    /// Removes tags from a project.
    /// </summary>
    Task<ApiResponseDto<ProjectTagDto>> RemoveTagsFromProjectAsync(Guid projectId, RemoveProjectTagsDto dto);

    /// <summary>
    /// Creates a new global Project Tag.
    /// </summary>
    Task<ApiResponseDto<ProjectTagDto>> CreateProjectTagAsync(CreateProjectTagDto dto);

    /// <summary>
    /// Deletes a global Project Tag.
    /// </summary>
    Task<ApiResponseDto<SimpleResponseDto>> DeleteProjectTagAsync(RemoveProjectTagsDto dto);
}