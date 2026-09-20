using Gadema.Api.Services.Base.Projects;
using Gadema.Core.Dtos.Base.Projects;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Base.Projects;

[ApiController]
[Route("api/v1/project-series")]
public class ProjectSeriesController : ControllerBase
{
    private readonly ProjectSeriesService _service;

    public ProjectSeriesController(ProjectSeriesService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() 
        => Ok(await _service.GetSeriesAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) 
        => Ok(await _service.GetSeriesAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProjectSeriesCreateDto dto)
    {
        var result = await _service.CreateSeriesAsync(dto);
        return result.Successful ? CreatedAtAction(nameof(GetById), new { id = result.Data.EntityId }, result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProjectSeriesUpdateDto dto) 
        => Ok(await _service.UpdateSeriesAsync(id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) 
        => Ok(await _service.DeleteSeriesAsync(id));
}