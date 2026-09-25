namespace Gadema.Core.Models.Base;

/// <summary>
/// Interface to mark an entity as supporting soft deletion.
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// Indicates whether the entity has been deleted.
    /// </summary>
    bool IsDeleted { get; set; }
}
