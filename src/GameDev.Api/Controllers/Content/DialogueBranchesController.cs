// =============================================================================
using Microsoft.AspNetCore.Http;
// GameDev.Api - ASP.NET Core Web API Controllers
// =============================================================================

using System;
using System.Threading.Tasks;
using GameDev.Api.Services;
using GameDev.Core.Dtos.DialogueTrees;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameDev.Api.Controllers;

/// <summary>
/// Controller for dialogue tree endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{id}/dialogue/branches")]
public class DialogueBranchesController : ControllerBase
{
    private readonly IDialogueService _dialogueService;
    private readonly ILogger<DialogueBranchesController> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public DialogueBranchesController(IDialogueService dialogueService, ILogger<DialogueBranchesController> logger)
    {
        _dialogueService = dialogueService;
        _logger = logger;
    }

    /// <summary>
    /// List dialogue branches.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetBranchesAsync(Guid id)
    {
        return Ok(await _dialogueService.GetBranchesAsync(id));
    }

    /// <summary>
    /// Create new dialogue branch.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateBranchAsync(Guid id, [FromBody] CreateBranchDto createDto)
    {
        return Ok(await _dialogueService.CreateBranchAsync(id, createDto));
    }
}