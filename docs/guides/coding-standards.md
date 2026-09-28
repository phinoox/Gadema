# 🛠️ GaDeMa Coding Standards & Patterns

This guide is the **Single Source of Truth** for implementation within the GaDeMa project. It is designed as a high-density "Cheat Sheet" to ensure consistency, prevent ambiguity, and maintain our architectural integrity.

---

## 📂 1. The Core Rule: Domain Clustering & Naming

To avoid name collisions (e.g., with `System.Threading.Task`) and maintain organization, we use **Domain Clustering**.

### 📏 Naming Conventions
| Type | Pattern | ✅ Correct Example | ❌ Avoid! |
| :--- | :--- | :--- | :--- |
| **Models** | `EntityName` (PascalCase) | `ProjectTask.cs` | `Task.cs` |
| **DTOs (Create)** | `CreateXDto` | `ProjectTaskCreateDto` | `TaskCreateDto` |
| **DTOs (Update)** | `UpdateXDto` | `ProjectTaskUpdateDto` | `TaskUpdateDto` |
| **DTOs (Response)**| `ResponseXDto` | `ProjectTaskResponseDto`| `TaskResponseDto` |
| **Controllers** | `ResourceNameController` | `TasksController` | `TaskController` |
| **Services** | `XxxService` | `ProjectTaskService` | `TaskService` |

### 📁 Folder Structure (Domain-Aware)
All files must be clustered by their domain:
```text
src/Gadema.Core/Models/Tasks/          <-- Models
src/Gadema.Core/Dtos/Tasks/            <-- DTOs
src/Gadema.Api/Controllers/Tasks/      <-- Controllers
src/Gadema.Api/Services/Tasks/         <-- Services
```

---

## 🎮 2. The Controller Pattern: "Thin Controllers"

Controllers should be minimal routing and dispatching layers. They should contain **zero** business logic and **zero** database access.

### ✅ Correct Implementation (One-Liner)
For simple retrieval or actions, use the `Ok(await ...)` pattern:
```csharp
public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] Guid? projectId) 
    => Ok(await _projectService.GetMatchesAsync(query, projectId));
```

---

## 🛡️ 3. The Service Pattern: Authorization & Logic

The Service layer is the "Muscle." It handles all business rules, permission checks, and data integrity.

### ✅ Correct Implementation (Access & Auth)
Always check for identity and permissions at the start of your service method using the `ApiResponseDto` pattern.

```csharp
public async Task<ApiResponseDto<ProjectResponseDto>> GetProjectAsync(Guid projectId)
{
    // 1. Check Authentication
    var loggedIn = await CheckIsLoggedIn();
    if (!loggedIn)
        return ApiResponseDto<ProjectResponseDto>.Unauthorized("you need to be logged in");

    // 2. Check Authorization (Scope-based)
    var authorizationError = await CheckAccessAsync<ProjectResponseDto>(projectId, Permission.CanView);
    if (authorizationError != null) 
        return authorizationError; // Return the error object directly

    // 3. Proceed with Business Logic
    var project = await _context.Projects.FindAsync(projectId);
    return ApiResponseDto<ProjectResponseDto>.Success(project);
}
```

---

## 🔄 4. The Hybrid Response Pattern

To provide a consistent API experience, we distinguish between **Data Retrieval** and **Action Confirmation**.

### 🧪 RAW Pattern (For Data Retrieval)
Used for `GET` requests where the goal is simply to return data.
```csharp
// Returns the raw object/collection directly in an Ok() wrapper
return Ok(await _service.GetAsync(id)); 
```

### 📦 WRAPPED Pattern (For Actions & Errors)
Used for `POST`, `PUT`, and `DELETE`. Every action must return a success/failure status.

**Success Example:**
```csharp
// Returns a wrapped confirmation of the action
return Ok(new {
    success = true,
    message = "Task created successfully",
    data = new { id = newTask.Id } 
});
```

**Error Example:**
```csharp
// Returns a wrapped error object
return Ok(ApiResponseDto<CreateResponseDto>.Failure("Description is required"));
```

---

## ⚙️ 5. Data Integrity & Performance

### ✅ Validation: DTO vs. Model
- **DTOs**: Must contain all validation attributes (`[Required]`, `[MaxLength]`, etc.).
- **Models**: Should only contain structural constraints via the Fluent API in the configuration files. **Never put validation logic on Models.**

### ✅ Eager Loading (Preventing N+1)
Always use `.Include()` when fetching entities that have related data to avoid multiple database roundtrips.
```csharp
// ✅ CORRECT: Single query with eager loading
var tasks = await _context.ProjectTasks
    .Include(t => t.Comments)
    .ToListAsync();

// ❌ INCORRECT: N+1 problem (querying comments in a loop)
foreach(var task in tasks) {
    task.Comments = await _context.Comments.Where(c => c.TaskId == task.Id).ToListAsync();
}
```

### ✅ Pagination Rules
All list endpoints must be paginated to prevent performance degradation.
- **Default Page Size**: 20
- **Maximum Page Size**: 100
- **Pattern**: Always provide `TotalCount` and `TotalPages`.

---
*Compiled from the GaDeMa Coding Guidelines Repository.*
