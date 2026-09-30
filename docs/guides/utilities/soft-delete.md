# Soft Delete Pattern

This guide explains the implementation of the **Soft Delete** pattern within GaDeMa, which allows entities to be logically removed from view without being physically deleted from the database. This preserves data integrity and provides a mechanism for audit trails or recovery.

## 1. The Interface (`ISoftDelete`)

Any entity that requires soft delete capabilities must implement the `ISoftDelete` interface found in `Gadema.Core.Models.Base`.

```csharp
public interface ISoftDelete
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity has been logically deleted.
    /// </summary>
    bool IsDeleted { get; set; }
}
```

**Note**: While the documentation summary previously mentioned `IsActive`, the actual implementation uses `IsDeleted`. Always follow the code.

## 2. Global Query Filtering

To ensure that "deleted" records do not appear in standard application queries, a **Global Query Filter** is automatically applied by the `GademaBaseContext`.

### How it works
The `ApplySoftDeleteFilters` method within `GademaBaseContext.OnModelCreating` scans all entity types during model initialization. If an entity implements `ISoftDelete`, it applies an EF Core global query filter: `e => e.IsDeleted == false`.

```csharp
// Logic applied automatically in GademaBaseContext
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
```

### Consequences of Global Filters
* **Automatic Filtering**: Every `DbSet<T>` query (e.g., `_context.Users.ToList()`) will automatically exclude any record where `IsDeleted == true`.
* **Bypassing the Filter**: If you explicitly need to include deleted items (for administrative or recovery purposes), use `.IgnoreQueryFilters()` in your LINQ query:
  ```csharp
  // Retrieves ALL users, including those logically deleted.
  var allUsers = await _context.Users.IgnoreQueryFilters().ToListAsync();
  ```

## 3. Implementation Checklist

When adding a new entity that should support soft deletion:
1. [ ] Implement the `ISoftDelete` interface on your model class.
2. [ ] Add the `IsDeleted` property to your model.
3. [ ] Ensure you use the `DbSet<T>` via `GademaBaseContext` so the filter is applied.

## ⚖️ Trade-offs

| Pros | Cons |
| :--- | :--- |
| **Data Recovery**: Allows for easy "undelete" operations. | **Storage Overhead**: Deleted records still occupy space in the database. |
| **Audit Trails**: Preserves history and relational integrity (prevents orphaned FKs). | **Index Bloat**: Large amounts of deleted data can slow down index scans if not managed. |
| **Relational Integrity**: Prevents breaking foreign key constraints by keeping the record present. | **Query Complexity**: Requires developers to be aware of `IgnoreQueryFilters()` for administrative tasks. |
