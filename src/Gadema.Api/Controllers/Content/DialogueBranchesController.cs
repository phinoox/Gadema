// =============================================================================
using Microsoft.AspNetCore.Http;
// Gadema.Api - ASP.NET Core Web API Controllers
// =============================================================================

using System;
using System.Threading.Tasks;
using Gadema.Api.Services;
using Gadema.Core.Dtos.DialogueTrees;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Gadema.Core.Dtos.Content.Branches;
using Gadema.Api.Services.Content;
using Gadema.Api.Services.DialogueTrees;

namespace Gadema.Api.Controllers;

/// <summary>
/// Controller for dialogue tree endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{id}/dialogue/branches")]
public class DialogueBranchesController : ControllerBase
{
    private readonly DialogueBranchService _dialogueService;
    private readonly ILogger<DialogueBranchesController> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public DialogueBranchesController(DialogueBranchService dialogueService, ILogger<DialogueBranchesController> logger)
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