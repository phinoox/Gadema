// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Identity;

/// <summary>
/// Controller for identity value management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/identity-values")]
public class IdentityValuesController : ControllerBase
{
    private readonly IIdentityValueService _identityValueService;
    private readonly ILogger<IdentityValuesController> _logger;

    public IdentityValuesController(IIdentityValueService identityValueService, ILogger<IdentityValuesController> logger)
    {
        _identityValueService = identityValueService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetIdentityValuesAsync(Guid projectId)
    {
        return Ok(await _identityValueService.GetIdentityValuesAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetIdentityValueAsync(Guid id)
    {
        return Ok(await _identityValueService.GetIdentityValueAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateIdentityValueAsync(Guid projectId, [FromBody] IdentityValueCreateDto createDto)
    {
        return Ok(await _identityValueService.CreateIdentityValueAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIdentityValueAsync(Guid id, [FromBody] IdentityValueUpdateDto updateDto)
    {
        return Ok(await _identityValueService.UpdateIdentityValueAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIdentityValueAsync(Guid id)
    {
        return Ok(await _identityValueService.DeleteIdentityValueAsync(id));
    }
}
