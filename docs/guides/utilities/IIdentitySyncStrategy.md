# IIdentitySyncStrategy Interface

The `IIdentitySyncStrategy` interface defines the contract for specialized modules responsible for the **Harmonization** process. It manages how updates to a resource's "Body" (its domain-specific data) are synchronized with its "Soul" (its core identity anchor).

## 1. Purpose & Role

In GaDeMa, an entity is composed of two parts: a stable **Anchor** (`MetaInfo`) and evolving **Domain Data**. When the domain data changes, the Anchor must be updated to reflect these changes (e.g., updating a Title or Slug).

`IIdentitySyncStrategy` provides a standardized way to perform this synchronization. It allows different types of content—such as Characters, Projects, or Tasks—to implement their own unique logic for how their identity is transformed during an update.

## 2. Interface Definition

```csharp
public interface IIdentitySyncStrategy
{
    /// <summary>
    /// Synchronizes the identity anchor with the provided update data.
    /// </summary>
    /// <typeparam name="T">The specific implementation type.</typeparam>
    /// <param name="identityId">The unique identifier of the MetaInfo anchor.</param>
    /// <param name="updateData">The new data to be applied and synchronized.</param>
    Task<bool> SyncAsync<T>(Guid identityId, BaseMetaInfoUpdateData updateData) where T : IIdentitySyncStrategy;
}
```

## 3. The Synchronization Workflow

This interface is part of the **Harmonization Protocol** used by `DomainService`. The workflow typically follows these steps:

1.  **Trigger**: A domain service receives an update request containing both new domain data and identity metadata (`BaseMetaInfoUpdateData`).
2.  **Dispatch**: The service calls `SyncIdentityAsync<T>(...)`, which routes the request to the appropriate implementation via the `MetadataService`.
3.s **Execution**: The strategy performs a "Delta Calculation" (determining what changed) and executes an atomic transaction that updates both the domain entity and its corresponding `MetaInfo` anchor.

## 4. Implementation Examples

Different content types require different synchronization logic. This is achieved by implementing specific strategies:

*   **`ContentIdentityStrategy`**: Mannages identity for general content items (e.g., Lore entries).
*   **`ProjectSeriesIdentityStrategy`**: Handles the complex hierarchy and identity requirements of a Project Series.
*   **`ProjectTaskIdentityStrategy`**: Manages the identity synchronization for task-specific metadata.

## ⚖️ Trade-offs

| Pros | Cons |
| :--- | :--- |
| **Decoupled Logic**: Separates the "how" of identity updates from the main domain business logic. | **Complexity**: Requires maintaining a mapping between content types and their respective strategies. |
| **Atomicity**: Ensures that identity anchors and domain data are updated in a single, consistent operation. | **Abstraction Overhead**: Adds an extra layer of indirection for developers tracing the flow of an update. |

***
*Last Updated: [Date]*