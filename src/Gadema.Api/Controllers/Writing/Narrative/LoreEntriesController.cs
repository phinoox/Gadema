// =============================================================================
using Gadema.Api.Services.Writing.Narrative;
using Gadema.Core.Dtos.Writing.Narrative;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Writing.Narrative;

/// <summary>
/// Controller for lore entry management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId:guid}/lore-entries")]
public class LoreEntriesController : ControllerBase
{
    private readonly LoreEntryService _loreEntryService;
    private readonly ILogger<LoreEntriesController> _logger;

    public LoreEntriesController(LoreEntryService loreEntryService, ILogger<LoreEntriesController> logger)
    {
        _loreEntryService = loreEntryService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetLoreEntriesAsync(Guid projectId)
    {
        return Ok(await _loreEntryService.GetLoreEntriesAsync(projectId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetLoreEntryAsync(Guid id)
    {
        return Ok(await _loreEntryService.GetLoreEntryAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateLoreEntryAsync(Guid projectId, [FromBody] LoreEntryCreateDto createDto)
    {
        return Ok(await _loreEntryService.CreateLoreEntryAsync(projectId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateLoreEntryAsync(Guid id, [FromBody] LoreEntryUpdateDto updateDto)
    {
        return Ok(await _loreEntryService.UpdateLoreEntryAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteLoreEntryAsync(Guid id)
    {
        return Ok(await _loreEntryService.DeleteLoreEntryAsync(id));
    }
}
