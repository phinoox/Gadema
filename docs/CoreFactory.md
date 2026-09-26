---

### 🛡️ Gadema Core Infrastructure: Documentation (v5)

This document outlines the architecture and intended usage of the `CoreServices` gateway, which provides access to all infrastructure-level operations in the system.

#### 1. Architectural Role: The "Gateway" Pattern

The `CoreServices` acts as a **centralized, type-safe gateway**. It does not implement business logic itself; instead, it aggregates specialized services (Audit, Metadata, Permissions) and provides access to them via a hierarchical structure.

This approach achieves:
* **Extreme Decoupling**: Domain services only depend on `ICoreServices`, not the concrete implementations of every utility.
* **Minimal Constructor Bloat**: Instead of injecting 10 different services into a domain service, you inject exactly one: `ICoreServices`.
* **Strongly Typed API**: Access is via clear, descriptive sub-gateways (e.g., `_core.Audit.LogAsync(...)`).

---

#### 2. The Core Hierarchy

All infrastructure operations are organized into four distinct pillars:

| Pillar | Interface | Primary Responsibility | Typical Operation |
| :--- | :--- | :--- | :--- |
| **Audit** | `IAuditService` | Recording business-level events. | `LogAsync(...)` |
| **Metadata** | `IMetadataService` | Managing "Soul" anchors (MetaInfo). | `CreateAsync(...)`, `ApplyUpdatesAsync(...)` |
| **Permissions** | `IPermissionEngine` | Evaluating granular access rights. | `CheckPermissionAsync(...)` |
| **Orchestrator** | `ICoreOrchestrator` | Coordinating multi-step workflows. | *To be implemented* |

---

#### 3. The "Soul & Body" Concept (Identity Sync)

A core requirement of the system is maintaining a consistent relationship between a domain entity (the **Body**) and its identity anchor, the `ContentMetaInfo` (the **Soul**). To manage this without cluttering domain services with infrastructure details, we use an **Identity Synchronization mechanism**.

##### The Implementation Flow:
1.  **Domain Service**: Expresses the intent to sync identity using a single call: `SyncIdentityAsync<T>(...)`.
2.            **Base Class (`DomainService`)**: Acts as a proxy, delegating the call to the `CoreServices` gateway.
3.            **Gateway (`CoreServices`)**: Routes the request to the `MetadataService`.
4.            **Infrastructure (`MetadataService`)**: Uses reflection to instantiate the specific strategy and executes the sync logic.

##### Example Usage in a Domain Service:
```csharp
public async Task<ApiResponseDto<CharacterResponseDto>> UpdateAsync(Guid id, CharacterUpdateDto dto)
{
    // 1. Perform domain-specific updates (The Body)
    var character = await _db.Characters.FindAsync(id);
    character.Name = dto.Name;

    // 2. Synchronize identity (The Soul)
    if (dto.ContentMetaInfo != null)
    {
        // Single, clean call that handles the complexity of strategy instantiation and execution
        var success = await SyncIdentityAsync<CharacterIdentityStrategy>(id, dto.ContentMetaInfo);
        if (!success) return ApiResponseDto<CharacterResponseDto>.ServerError("Sync failed.");
    }

    await _db.SaveChangesAsync();
    return ApiResponseDto<CharacterResponseDto>.Success(character);
}
```

---

#### 4. Dependency Flow (The "Golden Rule")

To prevent circular dependencies and maintain a clear hierarchy, all injections must follow this strict one-way flow:
**`Domain Service` $\rightarrow$ `ICoreServices` $\rightarrow$ `Atomic Services` & `ICoreOrchestrator`**

| Layer | Dependencies Allowed | Responsibility |
| :--- | :--- | :--- |
| **Domain Services** | `DbContext`, `ICoreServices` | Core business logic & validation. |
| **CoreFactory** | `Atomic Services` | Assembling complex entities from parts. |
| **Gateway (`CoreServices`)** | `All Atomic Services` & `ICoreOrchestrator` | Aggregating all infrastructure access. |
| **Atomic Services** | `DbContext` (only) | Single, discrete operations (CRUD/Logs). |

---

#### 5. Blind Spots & Design Considerations (Self-Reflection)

* **The "Context" Paradox**: While `CoreServices` provides a unified API, domain services still need their specific `DbContext` for primary business logic. The gateway does not replace the need for specialized context injection.
* **Complexity vs. Convenience**: We have successfully moved complex assembly and sync logic into the infrastructure layer while keeping the domain service API extremely clean and focused on business intent.

**End of Documentation (v5).**