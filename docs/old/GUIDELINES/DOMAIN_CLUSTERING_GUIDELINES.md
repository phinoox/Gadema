# 📁 Domain Clustering Guidelines for GaDeMa v0.1

This document defines the file organization structure per domain, ensuring consistency and maintainability across the codebase.

## File Organization Structure

### Model Files per Domain

```bash
src/
└── Gadema.Core/Models/
    ├── Authentication/
    │   ├── User.cs
    │   └── TeamMember.cs
    ├── Projects/
    │   └── Project.cs
    ├── Content/
    │   ├── MetaInfo.cs
    │   ├── StoryOutline.cs
    │   └── DialogueBranch.cs
    ├── Tasks/
    │   └── ProjectTask.cs
    └── [etc...]
```

### DTO Files per Domain

```bash
src/
└── Gadema.Core/Dtos/
    ├── Authentication/
    │   ├── SignInDto.cs
    │   ├── SignInWith2FADto.cs
    │   └── GoogleCallbackDto.cs
    ├── Projects/
    │   ├── CreateProjectDto.cs
    │   ├── UpdateProjectDto.cs
    │   └── ProjectResponseDto.cs
    ├── Content/
    │   ├── CreateMetaInfoDto.cs
    │   ├── UpdateMetaInfoDto.cs
    │   └── MetaInfoResponseDto.cs
    └── Tasks/
        ├── ProjectTaskCreateDto.cs
        ├── ProjectTaskUpdateDto.cs
        └── ProjectTaskResponseDto.cs
```

### Service Files per Domain

```bash
src/
└── Gadema.Api/Services/
    ├── Authentication/
    │   ├── ApiAuthService.cs
    │   ├── TwoFactorAuthService.cs
    │   └── TokenService.cs
    ├── Projects/
    │   └── ProjectService.cs
    ├── Content/
    │   └── MetaInfoService.cs
    ├── Tasks/
    │   └── ProjectTaskService.cs
```

### Controller Files per Domain

```bash
src/
└── Gadema.Api/Controllers/
    ├── Authentication/
    │   └── AuthController.cs
    ├── Projects/
    │   └── ProjectsController.cs
    ├── Content/
    │   └── MetaInfosController.cs
    ├── Tasks/
    │   └── TasksController.cs
```

### Configuration Files per Domain

```bash
src/
└── Gadema.Core/Configurations/
    ├── Authentication/
    │   ├── UserConfiguration.cs
    │   └── TeamMemberConfiguration.cs
    ├── Projects/
    │   └── ProjectConfiguration.cs
    ├── Content/
    │   ├── MetaInfoConfiguration.cs
    │   └── StoryOutlineConfiguration.cs
    ├── Tasks/
    │   ├── ProjectTaskConfiguration.cs
    │   └── TaskCommentsConfiguration.cs
```

---

## File Naming Conventions per Domain

### Entity Configuration Files

**Pattern**: `[EntityName]EntityTypeConfiguration`

```csharp
public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User> { }  // ✅ Correct
public class ProjectTaskEntityTypeConfiguration : IEntityTypeConfiguration<ProjectTask> { }  // ✅ Correct (updated from Task)
```

### DTO Files per Operation Type

| Operation Type | Naming Pattern | Example |
| :--- | :--- | :--- |
| **Create** | `Create[Entity]Dto` | `CreateProjectDto.cs`, `CreateMetaInfoDto.cs` |
| **Update** | `Update[Entity]Dto` | `UpdateProjectDto.cs`, `UpdateMetaInfoDto.cs` |
| **Response** | `[Entity]ResponseDto` | `ProjectResponseDto.cs`, `MetaInfoResponseDto.cs` |

### Controller Files

**Pattern**: `[Resource]Controller`

```csharp
public class ProjectsController : ControllerBase  // ✅ Correct
public class MetaInfosController : ControllerBase  // ✅ Correct
public class ProjectTasksController : ControllerBase  // ✅ Correct (updated from Task)
```

### Service Files

| Type | Naming Pattern | Example |
| :--- | :--- | :--- |
| **Implementation** | `[Entity]Service` | `ProjectTaskService.cs`, `MetaInfoService.cs` |
| **Interface** | `I[Entity]Service` | `IProjectTaskService.cs`, `IMetaInfoService.cs` |

---

## Cross-Domain Service Patterns

### When Services Span Multiple Domains

For services that need access to multiple domain entities, use a dedicated folder:

```bash
src/
└── Gadema.Api/Services/CrossDomain/
    ├── VersionControlService.cs  // Works with snapshots, tasks, content
    └── SearchService.cs  // Searches across all domains
```

### Configuration Files for Cross-Domain Services

```csharp
// ✅ CORRECT - Domain-aware cross-domain service configuration
public class MetaInfoEntityTypeConfiguration : IEntityTypeConfiguration<MetaInfo>
{
    public void Configure(EntityTypeBuilder<MetaInfo> builder)
    {
        // ... content configuration here
        
        // Include external references (cross-domain entity)
        builder.HasMany(ci => ci.ExternalReferences)
            .WithOne(er => er.ParentEntity)
            .HasForeignKey(er => er.ParentId);
    }
}
```

---

## Sub-Folder Organization for Complex Domains

### Content Domain Example

```bash
src/
└── Gadema.Core/Models/Content/
    ├── MetaInfo.cs
    ├── StoryOutline.cs
    ├── DialogueBranch.cs
    ├── ExternalReference.cs
    ├── MediaAttachment.cs
    ├── Tag.cs
    ├── ContentTags.cs
    └── MediaTags.cs
```

### DTO Sub-Folders per Operation Type

```bash
src/
└── Gadema.Core/Dtos/Content/
    ├── CreateDto/
    │   ├── CreateMetaInfoDto.cs
    │   └── CreateStoryOutlineDto.cs
    ├── UpdateDto/
    │   ├── UpdateMetaInfoDto.cs
    │   └── UpdateStoryOutlineDto.cs
    └── ResponseDto/
        ├── MetaInfoResponseDto.cs
        └── StoryOutlineResponseDto.cs
```

---

## Common Files per Domain

### Enums Folder (Shared Across Domains)

```bash
src/
└── Gadema.Core/Enums/
    ├── ContentTypeEnum.cs
    ├── ContentStatusEnum.cs
    ├── ViewModeEnum.cs
    ├── TaskDifficultyEnum.cs
    └── TaskStatusEnum.cs
```

### Shared DTOs (Common across all domains)

```bash
src/
└── Gadema.Core/Dtos/Common/
    ├── PaginationResult.cs
    └── ErrorResponseDto.cs
```

---

## Example Folder Tree for New Domain

When adding a new domain (e.g., `Inventory`):

```bash
src/
├── Gadema.Core/Models/Inventory/
│   ├── InventoryItem.cs
│   ├── EndingDefinition.cs
│   └── Enums/
│       └── ItemTypeEnum.cs
│
├── Gadema.Core/Dtos/Inventory/
│   ├── CreateDto/
│   │   └── CreateInventoryItemDto.cs
│   ├── UpdateDto/
│   │   └── UpdateInventoryItemDto.cs
│   └── ResponseDto/
│       └── InventoryItemResponseDto.cs
│
├── Gadema.Api/Controllers/Inventory/
│   └── InventoryItemsController.cs
│
├── Gadema.Api/Services/Inventory/
│   ├── InventoryService.cs
│   └── IInventoryService.cs
│
└── Gadema.Core/Configurations/Inventory/
    ├── InventoryItemConfiguration.cs
    └── EndingDefinitionConfiguration.cs
```

---

## Anti-Patterns to Avoid in File Organization

| Anti-Pattern | Example | ✅ Correct Approach |
| :--- | :--- | :--- |
| **Flat Structure** | All models in `Gadema.Core/Models/` without domain folders | ❌ Don't do this! Use domain folders |
| **Mixed DTOs** | CreatingDto.cs and UpdateDto.cs in same file | ✅ Separate files per operation type |
| **Generic Names** | Service.cs, Entity.cs (no context) | ✅ Descriptive names: InventoryService.cs |
| **Cross-Domain Mixing** | TaskConfiguration.cs with MetaInfo properties | ✅ Keep configurations domain-specific |

---

## Configuration File Naming per Domain

### Pattern: `[EntityName]EntityTypeConfiguration`

```csharp
// ✅ CORRECT - Follows naming convention
public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User> { }
public class ProjectTaskEntityTypeConfiguration : IEntityTypeConfiguration<ProjectTask> { }  // Updated from Task

// ❌ INCORRECT - Generic or inconsistent naming
public class TaskConfiguration : IEntityTypeConfiguration<Task> { }  // ❌ Should be ProjectTask now!
public class Entity1Configuration : IEntityTypeConfiguration<User> { }  // ❌ No context!
```

---

## Summary: Domain Clustering Checklist

| File Type | Must Cluster by Domain? | Priority | Example |
| :--- | :--- | :--- | :--- |
| **Models** | ✅ Yes | Critical | `Tasks/ProjectTask.cs` |
| **DTOs (Create)** | ✅ Yes | High | `Content/CreateDto/CreateMetaInfoDto.cs` |
| **DTOs (Update)** | ✅ Yes | High | `Projects/UpdateDto/UpdateProjectDto.cs` |
| **DTOs (Response)** | ✅ Yes | High | `Content/ResponseDto/MetaInfoResponseDto.cs` |
| **Services** | ✅ Yes | High | `Tasks/ProjectTaskService.cs` |
| **Controllers** | ✅ Yes | Critical | `Projects/ProjectsController.cs` |
| **Configurations** | ✅ Yes | High | `Tasks/ProjectTaskConfiguration.cs` |
| **Enums** | ⚠️ Optional (group by domain) | Medium | `Enums/ContentTypeEnum.cs` |
| **Common DTOs** | ✅ Yes (separate Common folder) | Low | `Dtos/Common/PaginationResult.cs` |

---

## Final Reminder

**Follow these domain clustering guidelines to maintain a clean, organized codebase!**

- ✅ Always organize files by domain in their respective folders
- ✅ Use consistent naming patterns per file type
- ✅ Keep configurations domain-specific
- ✅ Separate common/shared DTOs into their own folder
```
