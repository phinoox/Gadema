# IUserContext Interface

The `IUserContext` interface provides a mechanism for accessing information about the currently authenticated user within the execution context of a request. It acts as a transient identity provider that is populated during the authentication phase and consumed throughout the application lifecycle.

## 1. Purpose

In GaDeMa, identity is not just data; it's an active state used to drive security and auditing. `IUserContext` serves three primary roles:

* **Identity Verification**: Provides the `UserId` and `CurrentUser` entity for checking authentication status.
* **Authorization Context**: Supplies user-specific `Roles` used by the `IPermissionEngine` to evaluate granular permissions.
* **Audit Provenance**: Allows services (like `AuditService`) to record which user performed specific actions.

## 2. Interface Definition

```csharp
public interface IUserContext : IDisposable
{
    /// <summary>
    /// Gets or sets the current authenticated user entity.
    /// </summary>
    User? CurrentUser { get; set; }
    
    /// <summary>
    /// Gets or sets the unique identifier of the current user, 
    /// or null if not authenticated.
    /// </summary>
    Guid? UserId { get; set; }
    
    /// <summary>
    /// Gets or sets the hash of the API token used for authentication, if applicable.
    /// </summary>
    string? ApiTokenHash { get; set; }
    
    /// <summary>
    /// Gets or set the list of roles assigned to the current user for authorization checks.
    /// </summary>
    List<string> Roles { get; set; }
}
```

## 3. Lifecycle & Implementation

### The `UserContext` Implementation
The concrete implementation, `UserContext`, is registered as a **Scoped** service in the Dependency Injection container. This means:
1.  A new instance is created for every unique HTTP request/session.
2.  The instance is destroyed at the end of the request, ensuring no identity leakage between users.

### The Population Flow (Middleware)
The `AuthMiddleware` is responsible for populating this context. When a request arrives:
1.  The middleware validates the authentication token/credentials.
2.s If valid, it resolves the corresponding `User` from the database.
3.  It then sets the `UserId`, `CurrentUser`, and `Roles` on the scoped `IUserContext` instance for that specific request.

## 4. Consumption Patterns

### A. Authorization (The Permission Engine)
The `IPermissionEngine` consumes `IUserContext.Roles` to determine if a user has the necessary permissions to access a resource within a given scope (e.g., "Can this user edit this Project?").

### B. Auditing (The Audit Service)
Services like `AuditService` consume `IUserContext.UserId` and `ApiTokenHash` to provide immutable proof of who performed an action, ensuring high-integrity audit logs.

## 5. Testing Patterns

Testing identity requires different approaches depending on the test type.

### Unit Testing: Mocking Identity
For unit tests where you are testing a single service in isolation, use a mocking library (like Moq) to simulate the `IUserContext`. This is fast and does not require database access.

```csharp
// Example: Simulating an authenticated Admin user in a unit test
var mockUser = new User { Id = Guid.NewGuid(), UserName = "TestAdmin" };
var userContextMock = new Mock<IUserContext>();

userContextMock.SetupGet(x => x.UserId).Returns(mockUser.Id);
userContextMock.SetupGet(x => x.CurrentUser).Returns(mockUser);
userContextMock.SetupGet(x => x.Roles).Returns(new List<string> { "Admin" });

// Inject the mock into your service under test
var service = new ProjectService(..., userContextMock.Object);
```

### Integration Testing: `TestUserContextHelper`
For integration tests that run against a real (or in-memory) database, use the `TestUserContextHelper`. This helper automates the process of resolving services from your test factory and populating the context with an existing user from the database.

**Crucial Requirement:** 
The `TestUserContextHelper` relies on a user already existing in the database. You **must** ensure your test setup includes a seeding step that creates at least one active user before calling this helper to avoid identity mismatch errors during execution.

## ⚖️ Trade-offs

| Pros | Cons |
| :--- | :--- |
| **Decoupling**: Services depend on an interface, not the HTTP context, making them easier to unit test. | **State Management**: Requires careful management of the Scoped lifecycle to prevent stale data. |
| **Centralized Identity**: Provides a single source of truth for identity throughout a request. | **Dependency Requirement**: Most core services now implicitly depend on `IUserContext` being correctly populated by middleware. |

***
*Last Updated: [Date]*
