# Code Files Summary - Gadema Project

## Complete List of C# Files

### 📁 Gadema.Api (~42 files)

**Controllers:**
- Authentication/AuthController.cs
- Content/CommentsController.cs
- Content/ContentItemsController.cs
- Content/DialogueBranchesController.cs
- Content/ExternalReferencesController.cs
- Content/ReviewStatusController.cs
- Content/SearchController.cs
- Content/TagsController.cs
- Export/ExportController.cs
- Narrative/StorySequencesController.cs
- Projects/ProjectsController.cs
- Tasks/ProjectTaskController.cs

**Services:**
- Authentication/ApiAuthService.cs
- Content/ContentItemService.cs
- Content/DialogueService.cs
- Content/ExternalReferenceService.cs
- Content/TagService.cs
- EngineIntegration/ExportService.cs
- Narrative/StoryOutlineService.cs
- Projects/ProjectService.cs
- Tasks/CommentService.cs
- Tasks/ReviewStatusService.cs
- Tasks/TaskService.cs

**Other:**
- Program.cs
- ProjectInfo.cs
- Services/GlobalConfiguration.cs
- Services/Interfaces/IApiAuthService.cs
- Services/Interfaces/IContentService.cs
- Services/Interfaces/IProjectService.cs
- Services/Interfaces/IStoryOutlineService.cs
- Services/OtherInterfaces.cs
- Services/ServiceHelpers.cs

---

### 📁 Gadema.Core (~62 files)

**Models (31 files):**
- Abilities/AbilityDefinition.cs
- Abilities/AbilitySet.cs
- Activities/ActivityLog.cs
- Activities/TokenUsageLog.cs
- Attributes/AttributeDefinition.cs
- Attributes/AttributeSet.cs
- Attributes/CharacterAttributes.cs
- Attributes/ClassTemplate.cs
- Authentication/Team.cs
- Characters/CharacterBackground.cs
- Characters/CharacterDetails.cs
- Comment.cs
- Content/ContentItem.cs
- Content/DialogueBranch.cs
- Content/DialogueNode.cs
- EngineIntegration/AssetLink.cs
- EngineIntegration/EngineExportConfig.cs
- EngineIntegration/EngineFieldMapping.cs
- Identity/CharacterIdentity.cs
- Inventory/EndingDefinition.cs
- Narrative/LoreEntry.cs
- Projects/Project.cs
- Tasks/ProjectTask.cs
- Templates/AttributeSetDefinition.cs
- Templates/ProjectTemplate.cs

**Dtos (36 files):**
- Authentication/* (5)
- Comments/* (2)
- ContentItems/* (8)
- DialogueTrees/* (3)
- Export/* (5)
- ExternalReferences/* (2)
- Narrative/* (3)
- Projects/* (7)
- Reviews/* (2)
- Search/* (1)
- Tags/* (2)

**Enums (19 files):**
- ContentStatusEnum.cs, ContentTypeEnum.cs, ExportFormatEnum.cs
- IdentityTypeEnum.cs, LoreTypeEnum.cs, OutlineStatusEnum.cs
- OwnerTypeEnum.cs, ProjectDifficultyEnum.cs, ProjectStatusEnum.cs
- ProjectTemplateTypeEnum.cs, ProjectVisibilityEnum.cs, RelatedEntityTypeEnum.cs
- SnapshotTypeEnum.cs, TaskDifficultyEnum.cs, TaskPriorityEnum.cs
- TaskStatusEnum.cs, TeamMemberRoleEnum.cs, ViewModeEnum.cs

**Interfaces & Helpers:**
- IApiAuthService.cs, IContentService.cs, IProjectService.cs
- ServiceHelpers.cs, OtherInterfaces.cs

---

### 📁 Gadema.Data (1 file)
- GameDbContext.cs — EF Core DbContext with all entity configurations

---

### 📁 Gadema.Tests (~5 files)
- Integration/ApiIntegrationTests.cs
- Models/ModelValidationTests.cs
- Services/ContentItemServiceTests.cs
- Services/ServiceIntegrationTests.cs
- TestMetadata.cs

---

### 📁 Gadema.WebApp (~39 files)
- Components/Diologues/* (dialog components)
- Components/Layout/* (MainLayout, UserMenu, etc.)
- Components/Pages/* (Home, Content, Projects, Tasks, Dialogue, Tags, Reviews, Search, Settings, Signin, Register)
- AuthenticationStateProvider.cs

---

## Summary by Category:

| Project | Models | Dtos | Controllers | Services | Enums | Total |
|---------|--------|------|-------------|----------|-------|-------|
| **Gadema.Api** | — | 36 | 12 | 11 | — | ~59 |
| **Gadema.Core** | 31 | 36 | — | — | 19 | ~86 |
| **Gadema.Data** | — | — | — | — | — | 1 (DbContext) |
| **Gadema.WebApp** | — | — | — | — | — | ~40 |

**Total Source Files: ~235** (excluding obj/bin generated files)

---

## Architecture Overview

```
┌─────────────────┐     ┌──────────────────────┐     ┌─────────────────┐
│  Gadema.WebApp  │◄───►│   Gadema.Api         │◄───►│  Gadema.Core    │
│  (Blazor Server)│ HTTP│  (ASP.NET Core Web API)│ EF Core│  (Domain Models)│
└─────────────────┘     └────────────┬─────────┘     └─────────────────┘
         │                           │                        ▲
         ▼                          │                        │
    MudBlazor UI                   Database               Business Logic
```

**Key Principles:**
- **Core**: Pure domain models, DTOs, enums — no framework dependencies
- **Data**: EF Core DbContext with entity configurations (single file)
- **Api**: HTTP endpoints, service implementations, authentication middleware
- **WebApp**: Razor components, UI logic, HttpClient calls to Api layer

**Central Package Management:** `Directory.Build.props` defines all package versions at the solution root level.
