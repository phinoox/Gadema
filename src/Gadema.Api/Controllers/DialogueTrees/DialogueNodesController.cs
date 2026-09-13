// =============================================================================
using Gadema.Api.Services;
using Gadema.Api.Services.Content;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.DialogueTrees;

/// <summary>
/// Controller for dialogue node management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/dialogue-nodes")]
public class DialogueNodesController : ControllerBase
{
    private readonly DialogueNodeService _dialogueNodeService;
    private readonly ILogger<DialogueNodesController> _logger;

    public DialogueNodesController(DialogueNodeService dialogueNodeService, ILogger<DialogueNodesController> logger)
    {
        _dialogueNodeService = dialogueNodeService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetDialogueNodesAsync(Guid projectId)
    {
        return Ok(await _dialogueNodeService.GetDialogueNodesAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDialogueNodeAsync(Guid id)
    {
        return Ok(await _dialogueNodeService.GetDialogueNodeAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateDialogueNodeAsync(Guid projectId, [FromBody] DialogueNodeCreateDto createDto)
    {
        return Ok(await _dialogueNodeService.CreateDialogueNodeAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDialogueNodeAsync(Guid id, [FromBody] DialogueNodeUpdateDto updateDto)
    {
        return Ok(await _dialogueNodeService.UpdateDialogueNodeAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDialogueNodeAsync(Guid id)
    {
        return Ok(await _dialogueNodeService.DeleteDialogueNodeAsync(id));
    }
}
