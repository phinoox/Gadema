// =============================================================================
using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations;

namespace GameDev.Core.Dtos.ContentItems;

/// <summary>
/// DTO for content item list/response.
/// </summary>
public class ContentItemResponseDto
{
    /// <summary>
    /// Unique identifier for the content item.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Project ID.
    /// </summary>
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Content type.
    /// </summary>
    [Display(Name = "Content Type")]
    public int ContentType { get; set; }
    
    /// <summary>
    /// Title of the content item.
    /// </summary>
    [Display(Name = "Title")]
    public string Title { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the content item.
    /// </summary>
    [Display(Name = "URL Slug")]
    public string Slug { get; set; } = "";
    
    /// <summary>
    /// Short description for search/filtering.
    /// </summary>
    [Display(Name = "Short Description")]
    public string? ShortDesc { get; set; } = null!;
    
    /// <summary>
    /// Full description (Markdown/HTML).
    /// </summary>
    [Display(Name = "Description")]
    public string? Description { get; set; } = null!;
    
    /// <summary>
    /// Indicates if the content item is published.
    /// </summary>
    [Display(Name = "Published")]
    public bool Published { get; set; }
    
    /// <summary>
    /// Current status (Draft, InProgress, Published, Archived).
    /// </summary>
    [Display(Name = "Status")]
    public int Status { get; set; }
    
    /// <summary>
    /// View mode: PrivateWriting or Presentation.
    /// </summary>
    [Display(Name = "View Mode")]
    public string ViewMode { get; set; } = "";
    
    /// <summary>
    /// Current version number.
    /// </summary>
    public int Version { get; set; }
}