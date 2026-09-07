// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Narrative;

/// <summary>
/// Controller for lore entry management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/lore-entries")]
public class LoreEntriesController : ControllerBase
{
    private readonly ILoreEntryService _loreEntryService;
    private readonly ILogger<LoreEntriesController> _logger;

    public LoreEntriesController(ILoreEntryService loreEntryService, ILogger<LoreEntriesController> logger)
    {
        _loreEntryService = loreEntryService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetLoreEntriesAsync(Guid projectId)
    {
        return Ok(await _loreEntryService.GetLoreEntriesAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLoreEntryAsync(Guid id)
    {
        return Ok(await _loreEntryService.GetLoreEntryAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateLoreEntryAsync(Guid projectId, [FromBody] LoreEntryCreateDto createDto)
    {
        return Ok(await _loreEntryService.CreateLoreEntryAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLoreEntryAsync(Guid id, [FromBody] LoreEntryUpdateDto updateDto)
    {
        return Ok(await _loreEntryService.UpdateLoreEntryAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLoreEntryAsync(Guid id)
    {
        return Ok(await _loreEntryService.DeleteLoreEntryAsync(id));
    }
}
