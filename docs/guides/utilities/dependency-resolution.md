# Dependency Resolution & Seeding

The GaDeMa ecosystem relies on a robust mechanism for managing data dependencies during testing and initialization. This is achieved through two complementary components: the `DependencyResolver`, which calculates the topological order of entities, and the `DbSeeder`, which executes the actual creation process.

---

## 1. The Dependency Graph

In any relational database with Foreign Key (FK) constraints, data must be created in a specific order. You cannot create a "Child" entity until its "Parent" exists.

The `DependencyResolver` automs this by building a directed graph of your models based on the `[ModelDependency]` attribute.

### The `[ModelDependency]` Attribute
To participate in the dependency graph, a class must be decorated with this attribute, explicitly listing its required parent types.

```csharp
// Example: A Task depends on a Project
[ModelDependency(typeof(Project))]
public class ProjectTask : ISoftDelete 
{
    public Guid ProjectId { get; set; } // The FK that the resolver tracks
}
```

### Topological Sorting (Kahn's Algorithm)
The `DependencyResolver` uses **Kahn's Algorithm** to perform a topological sort. This process:
1.  Identifies "root" nodes (entities with zero dependencies).
2.  Iteratively removes these nodes from the graph and updates the in-degree of their neighbors.
3.  Produces a sorted list where every parent always appears before its children.

---

## 2. Automated Seeding (`DbSeeder`)

The `DbSeeder` is a utility used within integration tests to populate the database with valid, relational data without manual boilerplate.

### The `AutoSeed<T>` Workflow
When you call `DbSeeder.AutoSeed<T>(scope)`, the following sequence occurs:

1.  **Discovery**: It uses the `DependencyResolver` to find all required ancestors for type `T`.
2.  **Ordered Execution**: It iterates through the sorted list of ancestors, seeding each one first.
3.  **Instance Generation**: It uses **AutoFixture** to generate randomized but structurally valid data for every property not explicitly customized.
4.  **FK Wiring**: After a parent is seeded, it automatically maps the new parent's ID into the child's Foreign Key properties.

```csharp
// Example: Seed a task and its required project hierarchy in one call
var task = DbSeeder.AutoSeed<ProjectTask>(scope, t => {
    t.Title = "Target Task"; // Customizing specific fields
});
```

### The `Create<T>` vs `Seed<T>` Methods
* **`Create<T>(Action<T>? customize)`**: Geners an object in memory using AutoFixture. It does **not** touch the database. Useful for setting up local test data.
* **`Seed<T>(IServiceScope scope, T entity)`**: Persists a pre-built object into the database.

---

## 3. Summary of Workflow

| Step | Action | Responsibility |
| :--- | :--- | :--- |
| **1. Scan** | Build dependency graph via attributes | `DependencyResolver` |
| **2. Sort** | Generate topological order (Parents $\rightarrow$ Children) | Kahn's Algorithm |
| **3. Create** | Generate randomized data for non-required fields | `AutoFixture` |
| **4. Link** | Map Parent IDs to Child FK properties | `DbSeeder` (FK Wiring) |

## ⚖️ Trade-offs

| Pros | Cons |
| :--- | :--- |
| **Developer Velocity**: Eliminates the need for manual, order-sensitive setup code in tests. | **Complexity**: Requires maintaining the dependency graph via attributes; if an attribute is missing, seeding fails. |
| **Data Integrity**: Automatically ensures all Foreign Key constraints are satisfied during the seed process. | **Reflection Overhead**: The scanning and wiring processes use reflection, which adds overhead to test startup. |

***
*Last Updated: [Date]*
