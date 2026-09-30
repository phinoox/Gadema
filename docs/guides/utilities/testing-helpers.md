# Testing Helpers

Testing complex, permission-heavy logic requires a reliable way to simulate different user identities. The `Gadema.Tests.Helpers` namespace provides utilities to manage authentication state and identity context within integration tests.

---

## 1. Identity Mocking with `TestUserContextHelper`

In integration tests using `ApiWebApplicationFactory`, you need a way to tell the system "the current user is X" without going through the full HTTP middleware stack for every test case. The `TestUserContextHelper` automates this by injecting identity into the scoped `IUserContext`.

### How it Works
The helper retrieves the scoped `IUserContext` from your test factory and populates its properties (`UserId`, `CurrentUser`, and `Roles`) with a user retrieved from (or created in) the test database.

```csharp
// Example: Setting up an authenticated Admin context for an integration test
var scope = _factory.GetScope();
var owner = DbSeeder.Seed<User>(scope); // Seed a real user via DbSeeder
_userContext = TestUserContextHelper.CreateTestContext(_factory); 
// Now, all services in this scope see 'owner' as the current user.
```

### Critical Requirement: Database Seeding
The `TestUserContextHelper` relies on an existing identity in the database. To avoid "Identity Mismatch" errors (where a user exists in memory but not in the DB), you **must** ensure your test setup seeds at least one active user before calling this helper.

---

## 2. Testing Patterns

### Pattern A: Unit Testing (Isolated)
For unit tests where you are testing a single service in isolation, do not use the `TestUserContextHelper`. Instead, use a mocking library (like Moq) to mock the `IUserContext` interface directly. This is faster and more deterministic for logic-only tests.

```csharp
// Example: Mocking identity for a pure unit test
var user = new User { Id = Guid.NewGuid(), UserName = "TestUser" };
var mockContext = new Mock<IUserContext>();

mockContext.SetupGet(x => x.UserId).Returns(user.Id);
mockContext.SetupGet(x => x.CurrentUser).Returns(user);
mockContext.SetupGet(x => x.Roles).Returns(new List<string> { "Admin" });

// Inject the mock into your service
var service = new ProjectService(..., mockContext.Object);
```

### Pattern B: Integration Testing (Full Stack)
For integration tests that verify the interaction between services, middleware, and the database, use `TestUserContextHelper`. This ensures the identity flows through the actual dependency injection container used by your API.

---

## 3. Summary of Test Approaches

| Feature | Unit Testing (Mocking) | Integration Testing (Helper) |
| :--- | :--- | :--- |
| **Speed** | Extremely Fast | Slower (DB/DI overhead) |
| **Scope** | Single class in isolation | Full request pipeline |
| **Identity Source** | In-memory mock objects | Seeded database entities |
| **Primary Use Case** | Testing business logic rules | Testing permission gates & middleware |

## ⚖️ Trade-offs

| Approach | Pros | Cons |
| :--- | :--- | :--- |
| **Mocking** | High speed and total control over identity state. | Does not test the actual DI registration or Middleware population. |
| **TestUserContextHelper** | Tests the real authentication flow and DB constraints. | Requires database seeding and is subject to "identity leakage" if scopes are mismanaged. |

***
*Last Updated: [Date]*
