# 🛡️ Gadema Authorization Architecture: Documentation (v2)

## 1. Overview
The system uses a **Decoupled, Multi-Dimensional Authorization Engine**. Instead of hardcoded role checks inside services, we use a "Brain" (the `PermissionEngine`) that evaluates granular permissions against specific resource scopes via pluggable strategies.

This architecture separates **Identity** (who you are) from **Authorization** (what you can do), and allows for complex, context-aware decisions (e.g., *"Does this user have the 'Editor' role within this specific project?"*).

## 2. The Architectural Hierarchy
The system is organized into four distinct layers to ensure strict separation of concerns:

| Layer | Responsibility | Key Components |
| :--- | :--- | :--- |
| **The Law** | `GademaBaseContext` | Enforces global invariants (e.g., `ISoftDelete`) via Expression Trees. |
| **The Brain** | `IPermissionEngine` | Orchestrates decision-making by iterating through injected strategies. |
| **The Muscle** | `IPermissionStrategy` | Executes the actual data/role lookups (e.g., `ProjectRoleStrategy`). |
| **The Domain** | `DomainServices` | Performs business logic and calls the engine for access checks. |

---

## 3. The Request Lifecycle (The Four-Gate Flow)

Every request follows this sequence of gates to ensure security and data integrity:

1.  **Gate 1: Visibility Gate (Is it visible?)**
    *   Checks the `IsPublic` flag on the resource.
    *   If `true`, access is granted for **viewing only**.
2.  **Gate 2: Identity Gate (Who are you?)**
    *   Uses `IUserContext` to verify authentication.
    *   If anonymous and the resource is private $\rightarrow$ **Denied**.
3.  **Gate 3: Authorization Gate (Can you do this?)**
    *   Calls `IPermissionEngine.CheckPermissionAsync(...)`.
    *   The engine iterates through all registered `IPermissionStrategy` implementations until one returns `true`.
4.  **Gate 4: Execution Gate (The Work)**
    *   If authorized, the Service proceeds to call the `IIdentitySyncStrategy` to perform the atomic "Soul & Body" update.

---

## 4. Developer Guide: Extending the System

### A. Adding a New Permission
To add a new action (e.g., `CanPublish`), simply add it to the `Permission` enum in the core models.

```csharp
// src/Gadema.Core/Models/Base/Permissions/Permission.cs
public enum Permission {
    CanView, CanCreate, CanEdit, CanDelete, 
    CanPublish // New permission added here
}
```

### B. Implementing a New Strategy (The "How")
If you have new logic (e.g., checking if a user is part of a Guild), create a class implementing `IPermissionStrategy`.

1.  **Create the class**:
    ```csharp
    public class GuildPermissionStrategy : IPermissionStrategy {
        public async Task<bool> EvaluateAsync(Guid userId, Guid scopeId, Permission permission) {
            // Implement your custom logic here (e.g., query the database)
            return await _db.GuildMembers.AnyAsync(m => m.UserId == userId && m.GuildId == scopeId);
        }
    }
    ```
2.  **Register it in DI**: 
    In `DbInjection.cs`, add: `services.AddScoped<IPermissionStrategy, GuildPermissionStrategy>();`

### C. Using Permissions in a Service (The "Call")
To prevent "magic" and ensure visibility, always call the engine explicitly in your domain services.

```csharp
public async Task UpdateStoryAsync(Guid storyId, StoryUpdateDto dto) {
    // 1. Explicit Authorization Check
    bool isAuthorized = await _permissionEngine.CheckPermissionAsync(
        _userContext.CurrentUser.Id, 
        storyId, 
        Permission.CanEdit
    );

    if (!isAuthorized) throw new UnauthorizedAccessException();

    // 2. Proceed with Business Logic
    await _identitySyncStrategy.SyncAsync(storyId, dto);
}
```

---

## 5. Core Principles to Remember
* **The "Dumb" Engine**: The `PermissionEngine` should not know about databases or specific entities; it only orchestrates the strategies.
* **No Magic Extensions**: Avoid adding extension methods like `.HasPermission()` directly onto primitives (`Guid`). This keeps authorization explicit and easy to audit in the service layer.
* **The Law is Absolute**: Any entity implementing `ISoftDelete` must be filtered by the `GademaBaseContext` automatically via its expression tree implementation.

***

**End of Documentation (v2).**
