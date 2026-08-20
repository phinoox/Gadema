// =============================================================================
// ReferenceResponseDto - Response for a single external reference
// =============================================================================

using System.ComponentModel.DataAnnotations;

namespace GameDev.Core.Dtos.ExternalReferences;

/// <summary>
/// Single external reference response.
/// </summary>
public class ReferenceResponseDto
{
    public Guid Id { get; set; }
    
    public int ParentType { get; set; }
    
    public Guid? ParentId { get; set; }
    
    [Required, MaxLength(4096)]
    public string Url { get; set; } = "";
    
    [MaxLength(128)]
    public string Title { get; set; } = "";
    
    public int Type { get; set; }
    
    public bool IsActive { get; set; }
}
