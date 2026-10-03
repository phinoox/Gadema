This guide explains how to integrate and use the **GaDeMa Mocking Engine** within a client application (such as a Blazor WebApp or a Unit Test project).

---

# 📖 Usage Guide: Integrating the Mocking Engine

The GaDeMa Mocking Engine is designed to be "plug-and-play." It allows you to develop your frontend/client logic against a high-fidelity, stateful API simulation without needing a running backend.

## 1. The Core Concept: Dependency Injection
The engine is implemented as a `DelegatingHandler`. In .NET, this means it sits in the middle of the `HttpClient` pipeline. Instead of requests going out to the internet, they are intercepted by the mock engine.

## 2. Basic Setup (Client-Side)

To use the engine in your application, you must register the `MockHttpMessageHandler` within your `HttpClient` configuration.

### **In a Blazor/ASP.NET Core Environment**
In your `Program.cs`, configure your `HttpClient` to use the handler:

```csharp
// 1. Initialize the Mocking Infrastructure
// This calls our bootstrapper to populate RouteRegistry and MockRegistry
MockBootstrapper.Initialize(); 

// 2. Register the HttpClient with the Mock Handler
builder.Services.AddScoped(sp => 
{
    var handler = new MockHttpMessageHandler
    {
        InnerHandler = new HttpClientHandler() // The real network handler (unused)
    };

    return new HttpClient(handler)
    {
        BaseAddress = new Uri("https://api.gadema.com/") // Dummy base URL
    };
});
```

---

## 3. Simulating Real-World Scenarios

### **A. Simulating User Registration**
When your frontend calls the registration endpoint, the engine performs a "Double Write" (Anchor + Component).

**Client Code:**
```csharp
var registerDto = new RegisterDto { 
    UserName = "jdoe", 
    Email = "john@example.com", 
    Password = "Password123!" 
};

// This call triggers MockHttpMessageHandler -> AuthMockService.Create()
var response = await httpClient.PostAsJsonAsync("api/v1/auth/register", registerDto);
var authResult = await response.Content.ReadFromJsonAsync<AuthResponse>();
```

**What happens under the hood:**
1. The engine intercepts `POST /api/v1/auth/register`.
2. `AuthMockService` creates a `User` in `MockDataStore.Users`.
3. Simultaneously, it creates a `UserProfile` in `MockDataStore.UserProfiles` linked to that User ID.
4. A mock JWT is returned in the `AuthResponse`.

### **B. Simulating Profile Updates (Partial Updates)**
The engine uses the `ReflectionMapper` to simulate real `PATCH` or `PUT` behavior, allowing you to update only specific fields.

**Client Code:**
```csharp
var updateDto = new UserProfileUpdateDto { 
    Title = "New Professional Title",
    ShortDesc = "Updated bio text."
};

// This triggers MockHttpMessageHandler -> UserProfileMockService.Update()
var response = await httpClient.PutAsJsonAsync($"api/v1/users/{userId}/profile", updateDto);
```

**What happens under the hood:**
1. The engine extracts the `userId` from the URL.
2. It finds the existing profile in `MockDataStore.UserProfiles`.
3. It uses **Reflection** to copy only the non-null properties from your `UpdateDto` onto the existing object.

---

## 4. Testing with the Engine
The engine is highly effective for Integration Testing. You can write tests that verify how your UI reacts to different data states without a database.

```csharp
[Fact]
public async Task ProfileUpdate_UpdatesStoreCorrectly()
{
    // Arrange
    var client = CreateMockClient(); // Helper to get HttpClient with MockHandler
    var userId = Guid.NewGuid();
    // Pre-seed the store via internal access or a Seeder
    MockDataStore.UserProfiles.Add(new UserProfileResponseDto { UserId = userId, Title = "Old Title" });

    // Act
    var update = new UserProfileUpdateDto { Title = "New Title" };
    await client.PutAsJsonAsync($"api/v1/users/{userId}/profile", update);

    // Assert
    var updatedProfile = MockDataStore.UserProfiles.First(p => p.UserId == userId);
    Assert.Equal("New Title", updatedProfile.Title);
}
```

---

## 5. Summary of Capabilities

| Feature | Capability |
| :--- | :--- |
| **State Persistence** | Data stays in `MockDataStore` for the lifetime of the application session. |
| **Regex Routing** | Supports complex URL patterns and ID extraction. |
| **Scale-Invariant** | Automatically manages linked Identity/Profile entities during registration. |
| **Reflection-Driven** | Updates are applied intelligently via property matching, not manual mapping. |