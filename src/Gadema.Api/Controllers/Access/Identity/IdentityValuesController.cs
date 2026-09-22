using Gadema.Api.Services.Access.Identity;
using Gadema.Core.Dtos.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Identity;
[ApiController]
[Route("api/v1/projects/{projectId:guid}/identity-values")] // Hierarchy in route
public class IdentityValueController : ControllerBase
{
    private readonly IdentityValueService _service;

    public IdentityValueController(IdentityValueService service)
    {
        _service = service;
    }

    //definitionid as optionalfilter
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? definitionId) 
        => Ok(await _service.GetValuesAsync(definitionId ?? Guid.Empty));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) 
        => Ok(await _service.GetValueAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create(Guid projectId, [FromBody] IdentityValueCreateDto dto)
    {
        // The service will use the projectId from the route to validate access/linkage
        var result = await _service.CreateValueAsync(projectId, dto);
        return result.Successful ? CreatedAtAction(nameof(GetById), new { id = result.Data.EntityId }, result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid projectId, Guid id, [FromBody] IdentityValueUpdateDto dto) 
        => Ok(await _service.UpdateValueAsync(id, dto)); // Note: Service still needs the projectId for validation

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid projectId, Guid id) 
        => Ok(await _service.DeleteValueAsync(id));
}