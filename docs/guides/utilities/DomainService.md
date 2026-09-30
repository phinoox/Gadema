# DomainService Base Class

The `DomainService` is an abstract base class that serves as the foundation for all business logic implementations in GaDeMa. It provides a unified way to interact with core infrastructure engines and simplifies common tasks like authorization, auditing, and identity synchronization.

## 1. Role & Responsibility

A `DomainService` is responsible for implementing the actual "business rules" of a specific domain (e.g., managing tasks, handling characters, or processing stories). It sits between the API Controllers and the Data Access layer.

Rather than interacting with low-level engines like `IPermissionEngine` or `IAuditService` directly, domain services consume these capabilities through the `CoreServicesProvider`. This allows the base class to provide high-level "helper" methods that handle repetitive tasks such as mapping engine results into standardized API responses.

---

## 2. Relationship with CoreServicesProvider

The `DomainService` holds a reference to the `CoreServicesProvider`. This provider acts as a gateway, giving the service access to the core engines without requiring a massive list of individual dependencies in every constructor.

```csharp
public abstract class DomainService
{
    protected CoreServicesProvider _core; // The Gateway to all infrastructure
    // ...
}
```

---

## 3. Provided Helper Methods

To reduce boilerplate and ensure consistency, `DomainService` provides several high-level methods that subclasses can use to perform common operations.

### Authorization Helpers
These methods wrap the `IPermissionEngine` logic. They are responsible for both **Role Resolution** and **Response Mapping**.

* **`CheckAccessAsync<T>(Guid projectId, Permission permission, ...)`**: The primary method for checking if a user has permission to perform an action on a resource. 
    * **Role Resolution (Arrangement)**: If no specific role is provided, the service "arranges" the necessary data by mapping the abstract `Permission` to a concrete `ProjectMemberRoleEnum` (e.g., `CanEdit` $\rightarrow$ `Editor`). This translation is kept in the domain layer because it represents business-level logic rather than pure engine mechanics.
    * **Response Mapping**: It translates internal `AccessResult` statuses from the permission engine into standard `ApiResponseDto<T>` objects suitable for API consumers.
* **`IsAdminAsync(Guid projectId)`**: A convenience check to verify if the current user holds the `Admin` role for a specific project.

### Identity & Auditing Helpers
* **`CheckIsLoggedIn()`**: A quick check to see if the `IUserContext` contains an authenticated identity.
* **`LogDbAsync(...)`**: A high-level wrapper for the `IAuditService`, allowing developers to record business events (e.g., "Task Updated") with minimal code.
* **`SyncIdentityAsync<T>(...)`**: A wrapper for the `IMetadataService.SyncAsync` method, used to perform the "Harmonization" of a resource's identity.

---

## 4. Implementation Guidelines

When inheriting from `DomainService`, follow these principles:

1.  **Use Helpers First**: Always prefer the provided helper methods (like `CheckAccessAsync`) over calling the core engines directly. This ensures that your service adheres to the standard response patterns and role-mapping logic of the system.
2.  **Don't Bypass Authorization**: Every write operation or sensitive read should be preceded by an authorization check using the provided helpers.
3.  **Keep Logic in the Service**: The `DomainService` is where the complex business logic belongs. Keep controllers thin and move all validation, state management, and rule enforcement here.

## ⚖️ Trade-offs

| Pros | Cons |
| :--- | :--- |
| **Reduced Boilerplate**: Eliminates repetitive mapping and authorization code in every service. | **Inheritance Coupling**: Subclasses are tightly coupled to the base class and its dependency on `CoreServicesProvider`. |
| **Standardized Responses**: Ensures that all permission-based errors follow a consistent API format. | **Abstraction Overhead**: Can occasionally obscure the exact implementation of an underlying engine for developers debugging deep stack traces. |

***
*Last Updated: [Date]*