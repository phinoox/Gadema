# CoreServicesProvider (The Gateway)

`CoreServicesProvider` is a central container used to bundle all core infrastructure services. It acts as a **Gateway** for `DomainService` implementations, providing them with access to the cross-cutting engines required for auditing, authorization, metadata management, and identity tracking.

Instead of injecting multiple individual interfaces into every service constructor—which leads to "Constructor Bloat"—we inject this single provider.

## 1. Purpose & Design Pattern

The `CoreServicesProvider` implements a **Parameter Object** pattern (often referred to in this context as a **Gateway**) to manage the complexity of dependency injection for core services.

### Benefits:
* **Simplified Dependency Injection**: Reduces the number of parameters in domain service constructors, making them easier to read and maintain.
* **Unified API**: Provides a single, predictable entry point for all core infrastructure capabilities.
* **Decoupling**: Allows the implementation details of the core engines (like `IAuditService`) to evolve without requiring changes to every service that consumes them.

---

## 2. The Core Engines

The provider exposes access to four primary systems. These interfaces are designed to be "purely logical," meaning they focus on business rules and do not depend on web-specific contexts like `HttpContext` or `ApiResponseDto`.

### IAuditService
Responsible for recording business-level audit events and activity logs.
* `Task LogDbAsync(Guid? projectId, string action, string relatedEntityType, Guid? relatedEntityId = null, string? description = null)`

### IMetadataService
Manages the creation, updates, and synchronization of `MetaInfo` anchors (the "Soul" of entities).
* `Task<T> CreateAsync<T>(BaseMetaInfoCreateData createData, Action<T> initialize) where T : BaseMetaInfo`
* `Task ApplyUpdatesAsync(Guid metaInfoId, BaseMetaInfoUpdateData updateData)`
* `Task<bool> SyncAsync<T>(Guid identityId, BaseMetaInfoUpdateData updateData) where T : IIdentitySyncStrategy`
* `Task<bool> DeleteAsync<T>(Guid identityId) where T : BaseMetaInfo`

### IPermissionEngine
The central engine for evaluating granular, scope-based permissions.
* `Task<AccessResult> CheckAccessAsync(Guid userId, Guid projectId, ProjectMemberRoleEnum minRole)`

### IUserContext
Provides the identity and roles of the currently authenticated user within the request scope.
* `User? CurrentUser { get; set; }`
* `Guid? UserId { get; set; }`
* `string? ApiTokenHash { get; set; }`
* `List<string> Roles { get; set; }`

---

## 3. Integration with Domain Services

The `DomainService` acts as the primary consumer of this provider. It uses the gateway to wrap the complex engine logic into high-level, readable helper methods for domain developers.

**Implementation Reference:**
In a typical implementation, a `DomainService` will hold a reference to `CoreServicesProvider _core`. It then provides convenience wrappers such as:
* `LogDbAsync(...)`: A wrapper around `IAuditService`.
* `SyncIdentityAsync<T>(...)`: A wrapper around `IMetadataService`.
* `CheckAccessAsync<T>(...)`: A wrapper around `IPermissionEngine` that maps engine results to API response types.

## ⚖️ Trade-offs

| Pros | Cons |
| :--- | :--- |
| **Cleaner Constructors**: Prevents long parameter lists in domain services. | **Hidden Dependencies**: It can be harder to see at a glance exactly which specific sub-services a class depends on. |
| **Centralized Access**: Provides a single, predictable way to access all core infrastructure. | **Service Locator Risk**: If used for *all* services (not just domain services), it risks becoming an anti-pattern that hides architectural intent. |

***
*Last Updated: [Date]*