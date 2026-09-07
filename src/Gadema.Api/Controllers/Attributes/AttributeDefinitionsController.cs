// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Attributes;

/// <summary>
/// Controller for attribute definition management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/attribute-definitions")]
public class AttributeDefinitionsController : ControllerBase
{
    private readonly IAttributeDefinitionService _attributeDefinitionService;
    private readonly ILogger<AttributeDefinitionsController> _logger;

    public AttributeDefinitionsController(IAttributeDefinitionService attributeDefinitionService, ILogger<AttributeDefinitionsController> logger)
    {
        _attributeDefinitionService = attributeDefinitionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAttributeDefinitionsAsync(Guid projectId)
    {
        return Ok(await _attributeDefinitionService.GetAttributeDefinitionsAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAttributeDefinitionAsync(Guid id)
    {
        return Ok(await _attributeDefinitionService.GetAttributeDefinitionAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAttributeDefinitionAsync(Guid projectId, [FromBody] AttributeDefinitionCreateDto createDto)
    {
        return Ok(await _attributeDefinitionService.CreateAttributeDefinitionAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAttributeDefinitionAsync(Guid id, [FromBody] AttributeDefinitionUpdateDto updateDto)
    {
        return Ok(await _attributeDefinitionService.UpdateAttributeDefinitionAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAttributeDefinitionAsync(Guid id)
    {
        return Ok(await _attributeDefinitionService.DeleteAttributeDefinitionAsync(id));
    }
}
