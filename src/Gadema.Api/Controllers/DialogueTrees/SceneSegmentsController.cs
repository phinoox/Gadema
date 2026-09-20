using Gadema.Api.Services.Writing.Narrative;
using Gadema.Core.Dtos.Writing.DialogueTrees;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.DialogueTrees;

[ApiController]
[Route("api/v1/segments")]
public class SceneSegmentController : ControllerBase
{
    private readonly SceneSegmentService _service;

    public SceneSegmentController(SceneSegmentService service)
    {
        _service = service;
    }

    /// <summary>
    /// List segments. Use query params for projectId and sceneId to filter.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid projectId, [FromQuery] Guid? sceneId) 
        => Ok(await _service.GetSegmentsAsync(projectId, sceneId));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) 
        => Ok(await _service.GetSegmentByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromQuery] Guid projectId, [FromBody] SceneSegmentCreateDto dto)
    {
        var result = await _service.CreateSegmentAsync(projectId, dto);
        return result.Successful ? CreatedAtAction(nameof(GetById), new { id = result.Data.EntityId }, result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SceneSegmentUpdateDto dto) 
        => Ok(await _service.UpdateSegmentAsync(id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) 
        => Ok(await _service.DeleteSegmentAsync(id));
}