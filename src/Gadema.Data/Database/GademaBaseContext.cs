using System.Linq.Expressions;
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
        ApplySoftDeleteFilters(modelBuilder);
    }

    private void ApplySoftDeleteFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                // Build expression: e => e.IsDeleted == false
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
                var falseConstant = Expression.Constant(false);
                var equality = Expression.Equal(property, falseConstant);
                var lambda = Expression.Lambda(equality, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}