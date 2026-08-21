// =============================================================================
using System;
using System.ComponentModel.DataAnnotations;

namespace GameDev.Core.Dtos.ContentItems;

/// <summary>
/// Response data for content item autosave operations.
/// </summary>
public class ContentItemAutosaveResponseDto
{
    public Guid Id { get; set; }
    
    [Required, MaxLength(128)]
    public string Title { get; set; } = "";
    
    [MaxLength(4096)]
    public string ShortDesc { get; set; } = "";
    
    public int Version { get; set; }
    
    public DateTime LastModifiedAt { get; set; }
}

/// <summary>
/// Response data for version rollback operations.
/// </summary>
public class VersionInfo
{
    public Guid ContentItemId { get; set; }
    
    public int FromVersion { get; set; }
    
    public int ToVersion { get; set; }
    
    public bool Success { get; set; }
    
    public string? Message { get; set; }
}
