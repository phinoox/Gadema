using Microsoft.AspNetCore.Mvc;
using Gadema.Api.Services.Content;
using Gadema.Core.Dtos.ExternalReferences;

namespace Gadema.Api.Controllers.Base.MetaInfo;

[ApiController]
[Route("api/v1/content/external-references")]
public class ExternalReferencesController : ControllerBase
{
    private readonly ExternalReferenceService _service;

    public ExternalReferencesController(ExternalReferenceService service)
    {
        _service = service;
    }

    /// <summary>
    /// List all references. Use query parameters for filtering.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid projectId, [FromQuery] int? referenceType, [FromQuery] string? authorKeyword) 
        => Ok(await _service.GetReferencesAsync(projectId, referenceType, authorKeyword));

    /// <summary>
    /// Get a single reference by its ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) 
        => Ok(await _service.GetReferenceByIdAsync(id));

    /// <summary>
    /// Create a new reference within the specified project.
    /// </summary>
    [HttpPost("{projectId:guid}")]
    public async Task<IActionResult> Create(Guid projectId, [FromBody] ExternalReferenceCreateDto dto)
    {
        var result = await _service.CreateReferenceAsync(projectId, dto);
        return result.Successful ? CreatedAtAction(nameof(GetById), new { id = result.Data.EntityId }, result) : BadRequest(result);
    }

    /// <summary>
    /// Update an existing reference.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ExternalReferenceUpdateDto dto) 
        => Ok(await _service.UpdateReferenceAsync(id, dto));

    /// <summary>
        /// Delete a reference.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) 
        => Ok(await _service.DeleteReferenceAsync(id));
}