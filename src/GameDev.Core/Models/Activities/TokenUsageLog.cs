// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Represents a log entry for API token usage.
/// </summary>
public class TokenUsageLog
{
    /// <summary>
    /// Unique identifier for the token usage log entry.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the project token this usage belongs to.
    /// </summary>
    [Required]
    public Guid ProjectTokenId { get; set; }
    
    /// <summary>
    /// Client IP address (optional).
    /// </summary>
    [MaxLength(48)]
    public string? IPAddress { get; set; }  // Nullable for client IP
    
    /// <summary>
    /// User agent string.
    /// </summary>
    [MaxLength(512)]
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// Action type (Export, Read, Publish, etc.).
    /// </summary>
    public int Action { get; set; }  // Enum: Export, Read, Publish, etc.
    
    /// <summary>
    /// ID of the content item accessed (optional).
    /// </summary>
    public Guid? ContentId { get; set; }  // Nullable content item accessed
    
    /// <summary>
    /// Timestamp when the action occurred.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}