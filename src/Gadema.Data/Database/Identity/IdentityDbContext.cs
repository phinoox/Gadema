using Microsoft.EntityFrameworkCore;
using Gadema.Core.Models.Access;
using Gadema.Core.Models.Identity;

namespace Gadema.Data.Database.Identity;

/// <summary>
/// Manages user identity, permissions, and access control.
/// </summary>
public class IdentityDbContext : GademaBaseContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    // Ownership of Identity entities
    public DbSet<User> Users { get; set; }
    public DbSet<IdentityValue> IdentityValues { get; set; }
    public DbSet<CharacterIdentity> CharacterIdentities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply identity-specific configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }
}
