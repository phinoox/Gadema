using Gadema.Core.Models.Access;

namespace Gadema.Core.Interfaces;
public interface IUserContext : IDisposable
{
    // Current authenticated user (if any)
    User? CurrentUser { get; set; }
    
    // User ID as Guid or null if not authenticated
    Guid? UserId { get; set; }
    
    // Project token for API access (if applicable)
    string? ApiTokenHash { get; }
    
    // Roles for authorization checks
    List<string> Roles { get; }
    
    
}
