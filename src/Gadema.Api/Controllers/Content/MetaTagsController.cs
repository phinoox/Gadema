using Gadema.Api.Services.Tags;
using Gadema.Core.Models.Tags;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Tags;

[ApiController]
[Route("api/v1/metatags")]
public class MetaTagsController : ControllerBase
{
    private readonly MetaTagService _tagService;

    public MetaTagsController(MetaTagService tagService) => _tagService = tagService;

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _tagService.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] string name, [FromQuery] string? slug) 
        => Ok(await _tagService.CreateAsync(name, slug));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) 
        => Ok(await _tagService.DeleteAsync(id));
}