
using Gadema.Api.Services.Narrative;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Narrative;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Writing.Narrative;

/// <summary>
/// Controller for outline section management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId:guid}/outline-sections")]
public class OutlineSectionController : ControllerBase
{
    private readonly OutlineSectionService _outlineSectionService;
    private readonly ILogger<OutlineSectionController> _logger;

    public OutlineSectionController(OutlineSectionService outlineSectionService, ILogger<OutlineSectionController> logger)
    {
        _outlineSectionService = outlineSectionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetOutlineSectionsAsync(Guid projectId)
    {
        return Ok(await _outlineSectionService.GetOutlineSectionsAsync(projectId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOutlineSectionAsync(Guid id)
    {
        return Ok(await _outlineSectionService.GetOutlineSectionAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateOutlineSectionAsync(Guid projectId, [FromBody] OutlineSectionCreateDto createDto)
    {
        return Ok(await _outlineSectionService.CreateOutlineSectionAsync(projectId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateOutlineSectionAsync(Guid id, [FromBody] OutlineSectionUpdateDto updateDto)
    {
        return Ok(await _outlineSectionService.UpdateOutlineSectionAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteOutlineSectionAsync(Guid id)
    {
        return Ok(await _outlineSectionService.DeleteOutlineSectionAsync(id));
    }
}
