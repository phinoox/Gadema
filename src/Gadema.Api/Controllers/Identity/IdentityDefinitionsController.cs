// =============================================================================
using Gadema.Api.Services;
using Gadema.Api.Services.Identity;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Identity;

/// <summary>
/// Controller for identity definition management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/identity-definitions")]
public class IdentityDefinitionsController : ControllerBase
{
    private readonly IdentityDefinitionService _identityDefinitionService;
    private readonly ILogger<IdentityDefinitionsController> _logger;

    public IdentityDefinitionsController(IdentityDefinitionService identityDefinitionService, ILogger<IdentityDefinitionsController> logger)
    {
        _identityDefinitionService = identityDefinitionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetIdentityDefinitionsAsync(Guid projectId)
    {
        return Ok(await _identityDefinitionService.GetDefinitionsAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetIdentityDefinitionAsync(Guid id)
    {
        return Ok(await _identityDefinitionService.GetDefinitionAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateIdentityDefinitionAsync(Guid projectId, [FromBody] IdentityDefinitionCreateDto createDto)
    {
        return Ok(await _identityDefinitionService.CreateDefinitionAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIdentityDefinitionAsync(Guid id, [FromBody] IdentityDefinitionUpdateDto updateDto)
    {
        return Ok(await _identityDefinitionService.UpdateDefinitionAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIdentityDefinitionAsync(Guid id)
    {
        return Ok(await _identityDefinitionService.DeleteDefinitionAsync(id));
    }
}
