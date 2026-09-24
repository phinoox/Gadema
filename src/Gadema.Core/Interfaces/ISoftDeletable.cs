namespace Gadema.Core.Interfaces;


/// <summary>
/// Defines a contract for entities that support soft deletion.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
}