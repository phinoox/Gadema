// =============================================================================
// BranchResponseDto - Response for a single dialogue branch
// =============================================================================

using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.DialogueTrees;

/// <summary>
/// Single dialogue branch response.
/// </summary>
public class BranchResponseDto
{
    public Guid Id { get; set; }
    
    [Required, MaxLength(128)]
    public string Title { get; set; } = "";
    
    [MaxLength(128)]
    public string Slug { get; set; } = "";
    
    [MaxLength(1024)]
    public string? VisualNodeImageUri { get; set; }
    
    [MaxLength(512)]
    public string? CharacterIconUri { get; set; }
    
    public bool IsRoot { get; set; }
    
    public Guid? ParentNodeId { get; set; }
    
    public int OrderIndex { get; set; }
}
