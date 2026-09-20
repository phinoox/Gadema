// =============================================================================
using Gadema.Api.Services.Writing.Narrative;
using Gadema.Core.Dtos.Writing.DialogueTrees;
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
        return Ok(await _dialogueBranchService.GetBranchesAsync(projectId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDialogueBranchAsync(Guid id)
    {
        return Ok(await _dialogueBranchService.GetBranchAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateDialogueBranchAsync(Guid projectId, [FromBody] DialogueBranchCreateDto createDto)
    {
        return Ok(await _dialogueBranchService.CreateBranchAsync(projectId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateDialogueBranchAsync(Guid id, [FromBody] DialogueBranchUpdateDto updateDto)
    {
        return Ok(await _dialogueBranchService.UpdateBranchAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDialogueBranchAsync(Guid id)
    {
        return Ok(await _dialogueBranchService.DeleteBranchAsync(id));
    }
}
