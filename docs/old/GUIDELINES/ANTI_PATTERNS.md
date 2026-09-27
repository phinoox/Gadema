# 🚫 Anti-Patterns Reference

This document consolidates all "what NOT to do" guidance currently scattered across multiple docs.

## No Repository Pattern

❌ **INCORRECT** - Unnecessary abstraction for MVP:
```csharp
public interface IRepository<T> where T : class {}
public class MetaInfoRepository : IRepository<MetaInfo> {}
```

✅ **CORRECT** - Direct EF Core access:
```csharp
public class MetaInfoService
{
    private readonly GameDbContext _context;
    
    public async Task<MetaInfo> GetAsync(Guid id)
    {
        return await _context.MetaInfos.FindAsync(id);
    }
}
```

## No Hierarchical ProjectTasks

❌ **INCORRECT** - Complex epics/stories hierarchy:
```csharp
public class Epic { }  // ❌ Overkill for MVP
public class Story {}
public class TaskItem {}
```

✅ **CORRECT** - Flat structure with tags:
```csharp
public class ProjectTask
{
    public int Difficulty { get; set; }  // Easy, Medium, Hard
    public bool IsQuickWin { get; set; }  // ADHD-friendly
}
```

## No Magic Numbers

❌ **INCORRECT** - Hardcoded values:
```csharp
if (task.Difficulty == 1)  // ❌ What does 1 mean?
{
    // Should use Enum.Parse<TaskDifficulty>(...)
}
```

✅ **CORRECT** - Using enums:
```csharp
public class ProjectTask
{
    public TaskDifficulty Difficulty { get; set; }  // Easy, Medium, Hard
}
```

## No Over-Documentation for Simple CRUD

❌ **INCORRECT** - Excessive XML docs:
```csharp
/// <summary>Creates a content item...</summary>
/// <param name="dto">DTO object with required fields</param>
public async Task<MetaInfo> CreateAsync(CreateMetaInfoDto dto) {}  // ❌ Too much!
```

✅ **CORRECT** - Self-explanatory code:
```csharp
public async Task<MetaInfo> CreateAsync(CreateMetaInfoDto dto, Guid projectId)
{
    var item = new MetaInfo { ProjectId = projectId };  // Simple and clear
}
```

## No N+1 Queries Without Include()

❌ **INCORRECT** - Querying navigation properties in loop:
```csharp
foreach (var item in MetaInfos)
{
    var attachments = await _context.MediaAttachments
        .Where(a => a.MetaInfoId == item.Id).ToListAsync();  // ❌ N+1!
}
```

✅ **CORRECT** - Eager loading:
```csharp
return await _context.MetaInfos
    .Include(ci => ci.MediaAttachments)
    .ToListAsync();  // ✅ Single query!
```

## No Raw HTML in Description Fields

❌ **INCORRECT** - Storing raw HTML:
```csharp
public class MetaInfo
{
    public string Description { get; set; } = "<script>alert('XSS')</script>";  // ❌ Security risk!
}
```

✅ **CORRECT** - HTML escaping:
```csharp
if (!string.IsNullOrWhiteSpace(dto.Description))
{
    dto.Description = new HtmlEncoder().Encode(dto.Description);  // ✅ Safe!
}
```

## No Generic Exception Throwing

❌ **INCORRECT** - Generic exceptions:
```csharp
throw new Exception("Something went wrong");  // ❌ Too vague!
```

✅ **CORRECT** - Specific exceptions:
```csharp
throw new NotFoundException("User not found", userId);
throw new ValidationException(["Email required", "Password too short"]);
```

## No ModelState Check Before Processing

❌ **INCORRECT** - Skip validation:
```csharp
var item = await _context.MetaInfos.FindAsync(id);
item.Title = dto.Title;  // ❌ Process even if invalid!
```

✅ **CORRECT** - Always check first:
```csharp
if (!ModelState.IsValid)
{
    return BadRequest(new { success = false, errors });  // ✅ Validate first!
}
```

## No Pagination with Page Size > 100

❌ **INCORRECT** - Large page sizes:
```csharp
var items = await _context.MetaInfos.Skip(0).Take(500).ToListAsync();  // ❌ Too many!
```

✅ **CORRECT** - Limited pagination:
```csharp
if (pageSize < 1 || pageSize > 100) pageSize = 20;  // ✅ Validate range!
```

---

## Summary Checklist

| Anti-Pattern | Must Avoid? | Priority |
| :--- | :--- | :--- |
| Repository Pattern | ✅ Yes | Medium |
| Hierarchical Tasks (Epic/Story) | ✅ Yes | High |
| Magic Numbers | ✅ Yes | High |
| Over-Documentation | ✅ Yes | Low |
| N+1 Queries | ✅ Yes | Critical |
| Raw HTML in DB | ✅ Yes | Critical |
| Generic Exceptions | ✅ Yes | High |
| Skip ModelState Validation | ✅ Yes | Critical |
| Large Pagination Sizes | ✅ Yes | Medium |

---

## Final Reminder

**Follow these anti-patterns to maintain code quality and prevent common mistakes!**
```
