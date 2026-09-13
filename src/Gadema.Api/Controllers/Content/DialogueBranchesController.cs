// =============================================================================
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Gadema.Core.Dtos.DialogueTrees;
using Gadema.Api.Services.DialogueTrees;

namespace Gadema.Api.Controllers;

/// <summary>
/// Controller for managing the branching paths of dialogue trees.
/// </summary>
[ApiController]
[Route("api/v1/projects/{id:guid}/dialogue/branches")]
public class DialogueBranchesController : ControllerBase
{
    private readonly DialogueBranchService _dialogueService;
    private readonly ILogger<DialogueBranchesController> _logger;

    public DialogueBranchesController(DialogueBranchService dialogueService, ILogger<DialogueBranchesController> logger)
    {
        _dialogueService = dialogueService;
        _logger = logger;
    }

    /// <summary>
    /// List all dialogue branches for a specific project.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetBranchesAsync(Guid id)
    {
        _logger.LogInformation("Fetching dialogue branches for project {ProjectId}", id);
        return Ok(await _dialogueService.GetBranchesAsync(id));
    }

    /// <summary>
    /// Get a single dialogue branch by its ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBranchAsync(Guid id)
    {
        _logger.LogInformation("Fetching dialogue branch {BranchId}", id);
        return Ok(await _dialogueService.GetBranchAsync(id));
    }

    /// <summary>
    /// Create a new dialogue branch within a project.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateBranchAsync(Guid id, [FromBody] DialogueBranchCreateDto createDto)
    {
        _logger.LogInformation("Creating new dialogue branch in project {ProjectId}", id);
        return Ok(await _dialogueService.CreateBranchAsync(id, createDto));
    }

    /// <summary>
    /// Update an existing dialogue branch.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBranchAsync(Guid id, [FromBody] DialogueBranchUpdateDto updateDto)
    {
        _logger.LogInformation("Updating dialogue branch {BranchId}", id);
        return Ok(await _dialogueService.UpdateBranchAsync(id, updateDto));
    }

    /// <summary>
    /// Delete a dialogue branch.
    // </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBranchAsync(Guid id)
    {
        _logger.LogInformation("Deleting dialogue branch {BranchId}", id);
        return Ok(await _dialogueService.DeleteBranchAsync(id));
    }
}