This is a technical guide documenting the **Scale-Invariant Mocking Engine** architecture. It is designed for developers who need to extend the engine with new domain entities or understand how the simulation layer mimics the real API.

---

# 🛠️ GaDeMa Scale-Invariant Mocking Engine Guide

## 1. Architectural Philosophy: The Scale-Invariant Pattern
Unlike traditional mocking which treats every entity as a flat object, this engine implements the **Scale-Invariant Pattern**. This distinguishes between an **Anchor** (Core Identity) and a **Component** (Metadata/Profile).

*   **The Anchor**: A lean entity representing identity (e.g., `User`).
*   **The Component**: An extended metadata set linked via a shared ID (e.g., `UserProfile`).

This allows the system to scale by adding rich metadata without bloating the core identity objects, mirroring high-performance production architectures.

---

## 2. Core Components

### 🧩 `IMockService<TResponse>`
The heart of the engine. Every domain entity must implement this interface. It manages the full CRUD lifecycle within the mock environment.
*   `Create(object input)`: Handles instantiation and side-effects (like creating linked components).
*   `Get(Guid id)`: Retrieves an existing entity.
*   `Update(Guid id, object updateDto)`: Applies partial updates via reflection.
*   `Delete(Guid id)`: Removes the entity from the `MockDataStore`.

### 🗺️ `RouteRegistry`
A regex-based routing table that maps URL patterns to Input and Output DTOs.
*   **Pattern**: Uses `{id}` syntax (e.g., `api/v1/users/{id}/profile`).
*   **Resolution**: Converts OpenAPI-style paths into Regex to match incoming HTTP requests.

### 🔍 `MockRegistry`
A central service locator that maps DTO types to their respective `IMockService` implementations. It supports:
*   **Direct Registration**: Mapping a Response Type $\rightarrow$ Service.
*   **Mapping Registration**: Mapping an Input Type $\rightarrow$ Response Type (crucial for `POST/PUT` operations).

### 🚦 `MockHttpMessageHandler`
A `DelegatingHandler` that intercepts all outgoing HTTP requests. It acts as a **Generic Router**:
1.  Matches the URL against the `RouteRegistry`.
2.  Resolves the appropriate service via `MockRegistry`.
3.  Invokes the correct method (`Create`, `Get`, etc.) using reflection.

---

## 3. Developer Workflow: Adding a New Entity

To add a new domain (e.g., "Stories"), follow these steps:

### **Step 1: Define DTOs**
Create your Anchor and Component DTOs in `Gadema.Core.Dtos`.
*   `StoryResponseDto` (Anchor)
*   `StoryMetaInfoDto` (Component)

### **Step 2: Implement the Service**
Create a new class implementing `IMockService<StoryResponseDto>`.
```csharp
public class StoryMockService : IMockService<StoryResponseDto> {
    // Implement Create, Get, Update, Delete using MockDataStore.Stories
}
```

### **Step 3: Update the Data Store**
Add a collection to `MockDataStore.cs` to hold your new entities.
```csharp
public static List<StoryResponseDto> Stories { get; set; } = new();
```

### **Step 4: Register Everything**
In your application's startup/bootstrapper, register the routes and services:
```csharp
// 1. Define Routes
RouteRegistry.RegisterRoute<StoryCreateDto, StoryResponseDto>("api/v1/stories");

// 2. Define Services
var storyService = new StoryMockService();
MockRegistry.Register<StoryResponseDto>(storyService);
MockRegistry.RegisterMapping<StoryCreateDto, StoryResponseDto>(storyService);
```

---

## 4. Troubleshooting & Constraints

| Issue | Cause | Solution |
| :--- | :--- | :--- |
| `KeyNotFoundException` | Route or Service not registered. | Check `RouteRegistry` and `MockRegistry`. |
| `BadRequest` (Reflection) | Property name mismatch in DTOs. | Ensure `UpdateDto` properties match the Entity via `ReflectionMapper`. |
| `405 Method Not Allowed` | Verb not handled in `MockHttpMessageHandler`. | Verify the switch expression handles the HTTP method. |

## 5. Summary of Data Flow
**Request** $\rightarrow$ `MockHttpMessageHandler` $\rightarrow$ `RouteRegistry` (Finds Types) $\rightarrow$ `MockRegistry` (Finds Service) $\rightarrow$ `IMockService` (Executes Logic) $\rightarrow$ `MockDataStore` (State Update) $\rightarrow$ **JSON Response**.