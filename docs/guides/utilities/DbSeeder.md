# DbSeeder

The `DbSeeder` is a specialized utility used within the test suite to populate the database with predictable, valid data. It automs the complex task of creating entity graphs by resolving Foreign Key dependencies and managing object instantiation through **AutoFixture**.

## 1. Core Philosophy: Automation via AutoFixture

Instead of manually defining every property for every entity (which is error-prone and high-maintenance), `DbSeeder` uses **AutoFixture** to generate random, valid data for all non-required columns.

### The "Smart" Seeding Process
1.  **Instantiation**: Uses `_fixture.Create<T>()` to create an object with randomized but structurally valid properties.
2.  **Dependency Resolution**: Uses the `DependencyResolver` to identify all parent entities required by the target type.
3.s **Ordered Creation**: Seeds parents first, then children, ensuring that Foreign Key constraints are satisfied at every step.
4.  **FK Wiring**: Automatically maps the IDs from the newly created parent entities into the appropriate Foreign Key properties of the child entity.

---

## 2. Primary Methods

### `AutoSeed<T>(IServiceScope scope, ...)`
The most powerful method in the seeder. It performs a "deep seed" by following the dependency graph.

**Workflow:**
1.  Analy<zes the target type `T` using the `DependencyResolver`.
2.  Finds all required ancestors (parents) needed for `T`.
3.  Seeds each ancestor in topological order.
4.  Uses a custom `SetFKProperties` logic to wire up the navigation properties and FK IDs of the new instance.

```csharp
// Example: Automatically seeds a Task, including its Project parent
var task = DbSeeder.AutoSeed<ProjectTask>(scope); 
// Result: A Project is created first, then the task with its ProjectId set correctly.
```

### `Seed<T>(IServiceScope scope, T entity)`
Used for "shallow" seeding where you have already manually constructed an object (often using `DbSeeder.Create`) and want to persist it to the database.

### `Create<T>(Action<T>? customize = null)`
A helper that leverages AutoFixture to create a new instance of type `T` with randomized data, allowing for optional customization via a lambda expression. This is perfect for setting only the properties relevant to your specific test case.

```csharp
// Example: Create a User with a specific email but random everything else
var user = DbSeeder.Create<User>(u => u.Email = "special@example.com");
```

---

## 3. Advanced Features

### Enum Tag Seeding (`SeedEnumTags`)
Provides a way to seed the `MetaTag` table with entries derived from an enum. This is used to populate standard system tags (e.g., difficulty levels or story genres) that are defined as Enums in the core models.

### Intelligent Recursion Handling
The seeder includes custom AutoFixture behaviors to handle circular references and collection nesting:
*   **Omit on Recursion**: Prevents infinite loops when entities have bi-directional relationships (e.g., `Team` $\leftrightarrow$ `Member`).
*   **Domain Property Omitter**: Ensures that complex object collections are not automatically populated with unattached, "junk" data that would violate database constraints.

---

## 4. Summary of Seeding Modes

| Method | Mode | Use Case |
| :--- | :--- | :--- |
| **`Create<T>`** | In-Memory | Generating a single object for local setup/manipulation. |
| **`Seed<T>(scope, entity)`** | Manual Persist | Adding a pre-built object to the database. |
| **`AutoSeed<T>(scope)`** | Deep Seed | Creating an entire valid entity graph (Parent $\rightarrow$ Child) in one call. |

## ⚖️ Trade-offs

| Pros | Cons |
| :--- | :--- |
| **Developer Velocity**: Dramatically reduces the amount of setup code required for complex integration tests. | **Complexity**: The `DependencyResolver` and FK wiring logic add internal complexity to the test suite. |
| **Reliability**: Ensures that all relational constraints are met by following a strict topological order. | **Hidden Magic**: Over-reliance on `AutoSeed` can sometimes make it difficult for developers to see exactly what data is being used in a test. |

***
*Last Updated: [Date]*