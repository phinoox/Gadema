// =============================================================================
using Gadema.Api.Services.Content;
using Gadema.Api.Services.DialogueTrees;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.DialogueTrees;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.DialogueTrees;

/// <summary>
/// Controller for dialogue branch management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId:guid}/dialogue-branches")]
public class DialogueBranchesController : ControllerBase
{
    private readonly DialogueBranchService _dialogueBranchService;
    private readonly ILogger<DialogueBranchesController> _logger;

    public DialogueBranchesController(DialogueBranchService dialogueBranchService, ILogger<DialogueBranchesController> logger)
    {
        _dialogueBranchService = dialogueBranchService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetDialogueBranchesAsync(Guid projectId)
    {
        return Ok(await _dialogueBranchService.GetDialogueBranchesAsync(projectId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDialogueBranchAsync(Guid id)
    {
        return Ok(await _dialogueBranchService.GetDialogueBranchAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateDialogueBranchAsync(Guid projectId, [FromBody] DialogueBranchCreateDto createDto)
    {
        return Ok(await _dialogueBranchService.CreateDialogueBranchAsync(projectId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateDialogueBranchAsync(Guid id, [FromBody] DialogueBranchUpdateDto updateDto)
    {
        return Ok(await _dialogueBranchService.UpdateDialogueBranchAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDialogueBranchAsync(Guid id)
    {
        return Ok(await _dialogueBranchService.DeleteDialogueBranchAsync(id));
    }
}
