# 🛡️ Error Handling Guidelines for GaDeMa v0.1

This document standardizes exception types, error responses, and logging patterns across the application.

## Exception Type Hierarchy

### Base Exception Classes

```csharp
// ✅ CORRECT - Use specific exception types
public class NotFoundException : Exception 
{
    public NotFoundException(string entity, Guid id) 
        : base($"{entity} with ID {id} not found") {}
    
    // Usage: throw new NotFoundException("Character", characterId);
}

public class ValidationException : Exception
{
    public string[] Errors { get; set; }
    
    public ValidationException(IEnumerable<string> errors)
    {
        Errors = errors.ToArray();
    }
}

public class UnauthorizedException : Exception 
{
    public UnauthorizedException() 
        : base("Authentication required for this operation") {}
}

public class ForbiddenException : Exception 
{
    public ForbiddenException(string reason) 
        : base($"Access denied: {reason}") {}
}

public class BadRequestException : Exception
{
    public BadRequestException(string message) 
        : base(message) {}
    
    public BadRequestException(IEnumerable<string> errors, string message = null) 
        : base(message ?? "Bad request") { }
}

public class InternalServerException : Exception
{
    public InternalServerException(string message) 
        : base("Internal server error: " + message) {}
}
```

---

## Error Response Structure

### Success Response (200 OK / 201 Created)

```csharp
// ✅ CORRECT - Simple data retrieval or creation
{
    "id": "char-001",
    "title": "Geralt of Rivia",
    "slug": "geralt-of-rivia"
}

// ✅ CORRECT - Wrapped response for user-triggered actions
{
    "success": true,
    "message": "Content updated successfully",
    "data": {
        "id": "char-001",
        "title": "Geralt of Rivia"
    }
}
```

### Error Response (4xx)

```csharp
// ✅ CORRECT - Validation error response
{
    "success": false,
    "errors": ["Description cannot be empty", "Title is required"],
    "message": "Validation failed"
}

// ✅ CORRECT - Not found error
{
    "success": false,
    "errors": ["Character not found"],
    "message": "Resource not found"
}

// ✅ CORRECT - Unauthorized error
{
    "success": false,
    "errors": ["Authentication required"],
    "message": "Invalid or missing authentication token"
}
```

### Server Error (500 Internal Server Error)

```csharp
// ✅ CORRECT - Log internal error but show generic message to client
{
    "success": false,
    "errors": ["An unexpected error occurred"],
    "message": "Internal server error"
}
```

---

## Controller Error Handling Pattern

### Always Check ModelState First

```csharp
// ✅ CORRECT - Validate DTO before processing logic
[HttpPost]
public async Task<IActionResult> CreateContentAsync([FromBody] ContentItemDto dto)
{
    // Step 1: Always check validation first
    if (!ModelState.IsValid)
    {
        var errors = ModelState.Values.SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage);
        
        return BadRequest(new 
        {
            success = false,
            errors = errors,
            message = "Validation failed"
        });
    }

    // Step 2: Process business logic
    var item = new ContentItem 
    {
        ProjectId = dto.ProjectId,
        ContentType = dto.ContentType,
        Title = dto.Title,
        Description = dto.Description ?? ""
    };

    await _context.ContentItems.AddAsync(item);
    await _context.SaveChangesAsync();

    // Step 3: Return created item with appropriate response format
    return Ok(new 
    {
        id = item.Id,
        title = item.Title,
        description = item.Description
    });
}
```

### Handle Specific Exceptions Gracefully

```csharp
// ✅ CORRECT - Catch specific exceptions and map to appropriate responses
[HttpPost]
public async Task<IActionResult> CreateContentAsync([FromBody] ContentItemDto dto)
{
    try
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest(new { success = false, errors });
        }

        var item = await _contentService.CreateContentAsync(dto, dto.ProjectId);
        return Ok(item);
    }
    catch (NotFoundException ex) when (ex.InnerException != null)
    {
        // Handle related entity not found (e.g., project doesn't exist)
        return NotFound(ex.Message);  // "Project with ID X not found"
    }
    catch (ValidationException ex)
    {
        // Return validation errors in standard format
        return BadRequest(new 
        {
            success = false,
            errors = ex.Errors,
            message = "Validation failed"
        });
    }
    catch (UnauthorizedException ex)
    {
        // Authentication errors
        return Unauthorized(ex.Message);  // "Authentication required for this operation"
    }
    catch (Exception ex)
    {
        // Unhandled exceptions - log and show generic error
        _logger.LogError(ex, "An unexpected error occurred in CreateContentAsync");
        
        return StatusCode(500, new 
        {
            success = false,
            message = "Internal server error",
            errors = ["An unexpected error occurred"]
        });
    }
}
```

### Never Throw Generic Exceptions

```csharp
// ❌ INCORRECT - Don't throw generic exceptions
public async Task CreateContentAsync(ContentItemDto dto)
{
    if (dto.Title.Length > 100)
        throw new Exception("Title too long");  // ❌ Too vague!
}

// ✅ CORRECT - Use specific exception type
public async Task<ContentItem> CreateContentAsync(ContentItemDto dto)
{
    if (dto.Title.Length > 100)
        throw new BadRequestException("Title must be 100 characters or less");  // ✅ Specific!
}
```

---

## Validation Exception Patterns

### Multiple Validation Errors

```csharp
// ✅ CORRECT - Aggregate multiple validation errors
public async Task CreateContentAsync(ContentItemDto dto)
{
    var errors = new List<string>();

    if (string.IsNullOrWhiteSpace(dto.Title))
        errors.Add("Title is required");

    if (dto.Description == null || dto.Description.Length == 0)
        errors.Add("Description cannot be empty");

    if (!errors.Any()) return;  // All validations passed

    throw new ValidationException(errors);
}
```

### Error Message Formatting

```csharp
// ✅ CORRECT - Clear, user-friendly error messages
public class ContentItemDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(128, ErrorMessage = "Title must not exceed 128 characters")]
    public string Title { get; set; }

    [StringLength(4096, ErrorMessage = "Description must not exceed 4096 characters")]
    public string Description { get; set; }
}

// Usage in controller:
return BadRequest(new 
{
    success = false,
    errors = ["Title is required", "Description must not exceed 4096 characters"],
    message = "Validation failed"
});
```

---

## Logging Guidelines

### Log Internal Errors with Context

```csharp
// ✅ CORRECT - Log full exception details internally
try
{
    await _context.SaveChangesAsync();
}
catch (DbUpdateException ex) when (ex.InnerException != null)
{
    // Log the inner exception for debugging
    _logger.LogError(ex, "Database update failed. Inner exception: {InnerException}", 
        ex.InnerException.Message);
    
    // Throw with more context but less detail to client
    throw new InternalServerException("Database operation failed");
}

// ✅ CORRECT - Log sensitive data separately (not in exceptions)
_logger.LogInformation(
    "Content item created: Id={Id}, Title={Title}", 
    contentItem.Id, contentItem.Title);
```

### Never Log Sensitive Data

```csharp
// ❌ INCORRECT - Don't log sensitive information
_logger.LogError(ex, "User password: {Password}", user.Password);  // ❌ Security risk!

// ✅ CORRECT - Mask sensitive data
_logger.LogWarning("User registration failed for email: {Email}", 
    "user@example.com");  // ✅ Only log non-sensitive fields
```

---

## Error Response Translation (i18n)

### Standardized Error Messages per Language

```csharp
public static class ErrorMessageDictionary
{
    private static readonly Dictionary<string, string> EN = new()
    {
        ["Title is required"] = "Title is required",
        ["Description cannot be empty"] = "Description cannot be empty"
    };
    
    private static readonly Dictionary<string, string> ES = new()
    {
        ["Title is required"] = "Título es obligatorio",
        ["Description cannot be empty"] = "La descripción no puede estar vacía"
    };
}

// Usage in controller:
var errorKey = "Title is required";
var errorMessage = ErrorMessageDictionary.EN.TryGetValue(errorKey, out var message) 
    ? message 
    : ErrorMessageDictionary.ES[errorKey];  // Use locale-appropriate message

return BadRequest(new 
{
    success = false,
    errors = new[] { errorMessage },
    message = "Error"
});
```

---

## Health Check Error Handling

### Graceful Degradation

```csharp
// ✅ CORRECT - Return partial health status with warnings
[HttpGet("/health")]
public async Task<HealthCheckResponse> GetHealthAsync()
{
    try
    {
        var databaseHealthy = await IsConnectedToDatabase();
        return new HealthCheckResponse 
        {
            Status = databaseHealthy ? "Healthy" : "Degraded",
            DatabaseConnection = databaseHealthy,
            CacheStatus = "Unknown",  // Unknown when Redis not available
            MemoryUsageMB = Environment.WorkingSet / 1024 / 1024
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Health check failed");
        
        return new HealthCheckResponse 
        {
            Status = "Unhealthy",
            DatabaseConnection = false,
            CacheStatus = "Unknown"
        };
    }
}
```

---

## Summary: Error Handling Checklist

| Requirement | Must Implement? | Priority |
| :--- | :--- | :--- |
| Use specific exception types (not generic Exception) | ✅ Yes | Critical |
| Return standard error format for all errors | ✅ Yes | Critical |
| Check ModelState before processing logic | ✅ Yes | Critical |
| Log internal exceptions with full context | ✅ Yes | High |
| Never log sensitive data (passwords, tokens) | ✅ Yes | Critical |
| Handle NotFound/Unauthorized gracefully | ✅ Yes | High |
| Return user-friendly error messages | ✅ Yes | Medium |
| Support i18n for error messages | ⚠️ Optional | Low |

---

## Final Reminder

**Proper error handling makes your application robust and user-friendly!**

- ✅ Always use specific exception types (NotFoundException, ValidationException, etc.)
- ✅ Return consistent error format: `{ "success": false, "errors": [...], "message": "..." }`
- ✅ Check validation before processing business logic
- ✅ Log internal errors with full context, but never expose to clients
- ✅ Never log sensitive data (passwords, tokens, API keys)
```
