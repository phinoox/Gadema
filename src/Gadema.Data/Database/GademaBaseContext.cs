using Microsoft.EntityFrameworkCore;
using Gadema.Core.Models.Base;

namespace Gadema.Data.Database;

/// <summary>
/// The base contract for all DbContexts in the GaDeMa universe.
/// Enforces universal rules such as Soft Delete filters.
/// </summary>
public abstract class GademaBaseContext : DbContext
{
    protected GademaBaseContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ENFORCEMENT: Apply Global Query Filters for all entities implementing ISoftDelete.
        // This is a "Law" that cannot be easily bypassed by individual modules.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                // Note: In a real implementation, we would use an expression tree 
                // to create the HasQueryFilter lambda dynamically.
                // For now, this serves as the architectural placeholder for that logic.
            }
        }
    }



}