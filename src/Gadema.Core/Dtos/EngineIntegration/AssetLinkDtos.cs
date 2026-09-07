// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.EngineIntegration;

/// <summary>
/// Response DTO for an asset link.
/// </summary>
public class AssetLinkResponseDto
{
    public Guid Id { get; set; }
    public Guid MetaInfoId { get; set; }
    public string EnginePath { get; set; } = "";
    public string? EngineAssetId { get; set; }
    public string EngineFileType { get; set; } = "fbx";
}

/// <summary>
/// List response for asset links.
/// </summary>
public class AssetLinkListResponseDto
{
    public IEnumerable<AssetLinkResponseDto> Items { get; set; } = Enumerable.Empty<AssetLinkResponseDto>();
    public int TotalCount { get; set; }
}
