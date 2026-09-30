I have analyzed the `DependencyResolver` and its role in the system. 

The **Dependency Resolver** is a specialized utility designed to solve the "Topological Ordering" problem inherent in relational databases with Foreign Key dependencies. It allows the system to understand which entities must be created first (the "parents") before their dependent entities (the "children") can be instantiated.

Here is the documentation for the `DependencyResolver`.

---

# Dependency Resolver

The `DependencyResolver` provides a mechanism for calculating the correct order of operations when seeding or managing data with complex Foreign Key dependencies. It uses **Topological Sorting** to determine the sequence in which entities must be created to satisfy all relational constraints.

## 1. Core Concept: Topological Ordering

In any system with Foreign Key (FK) relationships, you cannot create a "Child" entity until its "Parent" exists. For example, you cannot create a `ProjectTask` until the `Project` it belongs to has been persisted in the database.

The `DependencyResolver` automs this by:
1.  **Scanning**: Examining classes for the `[ModelDependency]` attribute.
2.  **Graph Building**: Constructing a directed graph where an edge exists from a child to its parent (representing "must exist before").
3.  **Sorting**: Using **Kahn's Algorithm** to produce a list of types sorted such that all dependencies appear before the types that depend on them.

## 2. Implementation Details

### The `[ModelDependency]` Attribute
To participate in the dependency graph, a class must be decorated with the `[ModelDependency]` attribute, explicitly listing its required parent types.

```csharp
// Example: A Task depends on a Project
[ModelDependency(typeof(Project))]
public class ProjectTask : ISoftDelete 
{
    public Guid ProjectId { get; set; } // The FK that the resolver tracks
}
```

### The Resolution Process
The `ResolveDependencies` method performs the following steps:

1.  **Traversal**: It uses a queue-based traversal to build a map of all types and their defined dependencies.
2.  **Graph Construction**: It creates an adjacency list representing the directed edges between types.
3.  **Kahn's Algorithm**: 
    *   It identifies "root" nodes (types with zero in-degree/no dependencies).
    *   It iteratively removes these nodes from the graph and updates the in-degrees of their neighbors.
    *   The order of removal forms the topological sort.
4.  **Cycle Detection**: If any types remain unprocessed after the algorithm completes, a circular dependency (e.g., `A -> B -> A`) has been detected.

## 3. Key Methods

| Method | Purpose |
| :--- | :--- |
| `ResolveDependencies()` | Scans the core models assembly and returns the sorted list of types and any detected cycles. |
| `GetRequiredAncestors(Type target)` | Returns only the subset of types required to satisfy a specific target type, sorted by dependency order. This is used by the `DbSeeder` to perform targeted seeding. |
| `PrintHierarchy()` | A diagnostic utility that prints a visual tree representation of the dependency graph to the console. |

## 4. Usage in Data Seeding (`DbSeeder`)

The `DbSeeder` utilizes the `DependencyResolver` to automate complex data setup:

1.  **Targeted Discovery**: When `AutoSeed<T>` is called, it uses `GetRequiredAncestors(typeof(T))` to find every parent entity required for that specific object.
2.  **Ordered Execution**: The seeder iterates through the returned list, seeding each parent first.
3.  **Automatic FK Wiring**: After a parent is seeded, the resolver identifies which properties on the child are Foreign Keys and automatically pop{ulates them with the newly created parent's ID.

## ⚖️ Trade-offs

| Pros | Cons |
| :--- | :--- |
| **Automation**: Eliminates manual management of seeding order for complex graphs. | **Complexity**: Adds a layer of reflection-based logic that must be kept in sync with the actual database schema. |
| **Safety**: Detects circular dependencies at startup/test time rather than during runtime DB constraint errors. | **Overhead**: The reflection-heavy scanning is intended for development/testing and should not be used in high-frequency production paths. |

***
*Last Updated: [Date]*