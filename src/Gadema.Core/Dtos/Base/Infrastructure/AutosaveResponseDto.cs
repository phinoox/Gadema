/// <summary>
/// Response data containing the updated state after a content item has been automatically saved.
/// </summary>
public class MetaInfoAutosaveResponseDto
{
    /// <summary>
    /// The unique identifier of the meta-info record.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The updated title of the content item.
    /// </summary>
    [Required, MaxLength(128)]
    public string Title { get; set; } = "";
    
    /// <summary>
    /// The updated short description of the content item.
    /// </summary>
    [MaxLength(4096)]
    public string ShortDesc { get; set; } = "";
    
    /// <summary>
    /// The new version number resulting from the save operation.
    /// </summary>
    public int Version { get; set; }
    
    /// <summary>
    /// The timestamp of the last modification in UTC.
    /// </summary>
    public DateTime LastModifiedAt { get; set; }
}

/// <summary>
/// Response data indicating the result of a version rollback operation.
/// </summary>
public class VersionInfo
{
    /// <summary>
    /// The unique identifier of the meta-info record being rolled back.
    /// </summary>
    public Guid MetaInfoId { get; set; }
    
    /// <summary>
    /// The version number being rolled back from.
    /// </summary>
    public int FromVersion { get; set; }
    
    /// <summary>
    /// The target version number after the rollback.
    /// </summary>
    public int ToVersion { get; set; }
    
    /// <summary>
    /// Indicates whether the rollback operation was successful.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// An optional message providing details on the outcome of the operation.
    /// </summary>
    public string? Message { get; set; }
}
