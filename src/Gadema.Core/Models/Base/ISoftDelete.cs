namespace Gadema.Core.Models.Base;

/// <summary>
/// Defines a contract for entities that support soft deletion, allowing them to be
/// marked as deleted without being physically removed from the database.
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity has been logically deleted.
    /// </summary>
    bool IsDeleted { get; set; }
}

