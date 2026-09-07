// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Attributes;

/// <summary>
/// Controller for attribute set management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/attribute-sets")]
public class AttributeSetsController : ControllerBase
{
    private readonly IAttributeSetService _attributeSetService;
    private readonly ILogger<AttributeSetsController> _logger;

    public AttributeSetsController(IAttributeSetService attributeSetService, ILogger<AttributeSetsController> logger)
    {
        _attributeSetService = attributeSetService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAttributeSetsAsync(Guid projectId)
    {
        return Ok(await _attributeSetService.GetAttributeSetsAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAttributeSetAsync(Guid id)
    {
        return Ok(await _attributeSetService.GetAttributeSetAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAttributeSetAsync(Guid projectId, [FromBody] AttributeSetCreateDto createDto)
    {
        return Ok(await _attributeSetService.CreateAttributeSetAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAttributeSetAsync(Guid id, [FromBody] AttributeSetUpdateDto updateDto)
    {
        return Ok(await _attributeSetService.UpdateAttributeSetAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAttributeSetAsync(Guid id)
    {
        return Ok(await _attributeSetService.DeleteAttributeSetAsync(id));
    }
}
