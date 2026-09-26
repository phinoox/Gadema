namespace Gadema.Core.Models.Base.Permissions;

/// <summary>
/// Defines the set of granular actions that can be performed on resources.
/// </summary>
public enum Permission
{
    // Basic CRUD permissions
    CanView,
    CanCreate,
    CanEdit,
    CanDelete,
    
    // Specialized permissions (can be expanded by modules)
    CanPublish,
    CanManageTags,
    CanAssignRole
}
