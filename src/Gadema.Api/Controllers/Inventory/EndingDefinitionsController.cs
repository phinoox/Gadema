// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Inventory;

/// <summary>
/// Controller for ending definition management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/endings")]
public class EndingDefinitionsController : ControllerBase
{
    private readonly IEndingDefinitionService _endingDefinitionService;
    private readonly ILogger<EndingDefinitionsController> _logger;

    public EndingDefinitionsController(IEndingDefinitionService endingDefinitionService, ILogger<EndingDefinitionsController> logger)
    {
        _endingDefinitionService = endingDefinitionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetEndingDefinitionsAsync(Guid projectId)
    {
        return Ok(await _endingDefinitionService.GetEndingDefinitionsAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEndingDefinitionAsync(Guid id)
    {
        return Ok(await _endingDefinitionService.GetEndingDefinitionAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateEndingDefinitionAsync(Guid projectId, [FromBody] EndingDefinitionCreateDto createDto)
    {
        return Ok(await _endingDefinitionService.CreateEndingDefinitionAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEndingDefinitionAsync(Guid id, [FromBody] EndingDefinitionUpdateDto updateDto)
    {
        return Ok(await _endingDefinitionService.UpdateEndingDefinitionAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEndingDefinitionAsync(Guid id)
    {
        return Ok(await _endingDefinitionService.DeleteEndingDefinitionAsync(id));
    }
}
