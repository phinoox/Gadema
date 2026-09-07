// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Attributes;

/// <summary>
/// Controller for class template management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/class-templates")]
public class ClassTemplatesController : ControllerBase
{
    private readonly IClassTemplateService _classTemplateService;
    private readonly ILogger<ClassTemplatesController> _logger;

    public ClassTemplatesController(IClassTemplateService classTemplateService, ILogger<ClassTemplatesController> logger)
    {
        _classTemplateService = classTemplateService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetClassTemplatesAsync(Guid projectId)
    {
        return Ok(await _classTemplateService.GetClassTemplatesAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClassTemplateAsync(Guid id)
    {
        return Ok(await _classTemplateService.GetClassTemplateAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateClassTemplateAsync(Guid projectId, [FromBody] ClassTemplateCreateDto createDto)
    {
        return Ok(await _classTemplateService.CreateClassTemplateAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClassTemplateAsync(Guid id, [FromBody] ClassTemplateUpdateDto updateDto)
    {
        return Ok(await _classTemplateService.UpdateClassTemplateAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClassTemplateAsync(Guid id)
    {
        return Ok(await _classTemplateService.DeleteClassTemplateAsync(id));
    }
}
