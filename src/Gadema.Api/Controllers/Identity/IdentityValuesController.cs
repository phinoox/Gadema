// =============================================================================
using Gadema.Api.Services;
using Gadema.Api.Services.Identity;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Identity;

/// <summary>
/// Controller for identity value management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/identity-values")]
public class IdentityValuesController : ControllerBase
{
    private readonly IdentityValueService _identityValueService;
    private readonly ILogger<IdentityValuesController> _logger;

    public IdentityValuesController(IdentityValueService identityValueService, ILogger<IdentityValuesController> logger)
    {
        _identityValueService = identityValueService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetIdentityValuesAsync(Guid projectId)
    {
        return Ok(await _identityValueService.GetValuesAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetIdentityValueAsync(Guid id)
    {
        return Ok(await _identityValueService.GetValuesByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateIdentityValueAsync(Guid projectId, [FromBody] IdentityValueCreateDto createDto)
    {
        return Ok(await _identityValueService.CreateValueAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIdentityValueAsync(Guid id, [FromBody] IdentityValueUpdateDto updateDto)
    {
        return Ok(await _identityValueService.UpdateValueAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIdentityValueAsync(Guid id)
    {
        return Ok(await _identityValueService.DeleteValueAsync(id));
    }
}
