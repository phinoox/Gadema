using Microsoft.AspNetCore.Mvc;
using Gadema.Api.Services.Idendity; // Note: using your current namespace 'Idendity'
using Gadema.Core.Dtos.Identity;

namespace Gadema.Api.Controllers.Identity;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/identity-definitions")]
public class IdentityDefinitionController : ControllerBase
{
    private readonly IdentityDefinitionService _service;

    public IdentityDefinitionController(IdentityDefinitionService service)
    {
        _service = service;
    }

    /// <summary>
    /// List all identity definitions for a project.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid projectId) 
        => Ok(await _service.GetDefinitionsAsync(projectId));

    /// <summary>
    /// Get a single identity definition by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) 
        => Ok(await _service.GetDefinitionAsync(id));

    /// <summary>
    /// Create a new identity definition within the specified project.
    /// </summary>
    [HttpPost("{projectId:guid}")]
    public async Task<IActionResult> Create(Guid projectId, [FromBody] IdentityDefinitionCreateDto dto)
    {
        var result = await _service.CreateDefinitionAsync(projectId, dto);
        return result.Successful ? CreatedAtAction(nameof(GetById), new { id = result.Data.EntityId }, result) : BadRequest(result);
    }

    /// <summary>
    /// Update an existing identity definition.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] IdentityDefinitionUpdateDto dto) 
        => Ok(await _service.UpdateDefinitionAsync(id, dto));

    /// <summary>
    /// Delete an identity definition.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) 
        => Ok(await _service.DeleteDefinitionAsync(id));
}