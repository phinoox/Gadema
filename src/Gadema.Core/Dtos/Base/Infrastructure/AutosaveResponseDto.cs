// =============================================================================
using System;
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Base.Infrastructure;

/// <summary>
/// Response data for content item autosave operations.
/// </summary>
public class MetaInfoAutosaveResponseDto
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
    public Guid MetaInfoId { get; set; }
    
    public int FromVersion { get; set; }
    
    public int ToVersion { get; set; }
    
    public bool Success { get; set; }
    
    public string? Message { get; set; }
}
