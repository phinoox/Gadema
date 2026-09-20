// src/Gadema.Api/Controllers/WorldBuilding/FactionsController.cs
using Gadema.Api.Services.Writing.WorldBuilding;
using Gadema.Core.Dtos.Writing.WorldBuilding;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Writing.WorldBuilding;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/factions")]
public class FactionsController : ControllerBase
{
    private readonly FactionService _factionService;
    private readonly ILogger<FactionsController> _logger;

    public FactionsController(FactionService factionService, ILogger<FactionsController> logger)
    {
        _factionService = factionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetFactionsAsync(Guid projectId)
    {
        return Ok(await _factionService.GetFactionsAsync(projectId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetFactionAsync(Guid id)
    {
        return Ok(await _factionService.GetFactionAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateFactionAsync(Guid projectId, [FromBody] FactionCreateDto createDto)
    {
        return Ok(await _factionService.CreateFactionAsync(projectId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateFactionAsync(Guid id, [FromBody] FactionUpdateDto updateDto)
    {
        return Ok(await _factionService.UpdateFactionAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteFactionAsync(Guid id)
    {
        return Ok(await _factionService.DeleteFactionAsync(id));
    }
}