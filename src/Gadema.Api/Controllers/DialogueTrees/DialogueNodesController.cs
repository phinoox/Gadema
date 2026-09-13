using Microsoft.AspNetCore.Mvc;
using Gadema.Core.Dtos.DialogueTrees;
using Gadema.Api.Services.DialogueTrees;

namespace Gadema.Api.Controllers.DialogueTrees;

/// <summary>
/// Controller for dialogue node management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId:guid}/branches/{branchId:guid}/nodes")]
public class DialogueNodesController : ControllerBase
{
    private readonly DialogueNodeService _service;
    private readonly ILogger<DialogueNodesController> _logger;

    public DialogueNodesController(DialogueNodeService service, ILogger<DialogueNodesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetNodesAsync(Guid projectId, Guid branchId)
    {
        return Ok(await _service.GetNodesAsync(projectId, branchId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetNodeAsync(Guid projectId, Guid branchId, Guid id)
    {
        return Ok(await _service.GetNodeByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateNodeAsync(Guid projectId, Guid branchId, [FromBody] DialogueNodeCreateDto dto)
    {
        return Ok(await _service.CreateNodeAsync(projectId, branchId, dto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateNodeAsync(Guid projectId, Guid branchId, Guid id, [FromBody] DialogueNodeUpdateDto dto)
    {
        return Ok(await _service.UpdateNodeAsync(projectId, branchId, id, dto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteNodeAsync(Guid projectId, Guid branchId, Guid id)
    {
        return Ok(await _service.DeleteNodeAsync(id));
    }
}