# 📁 Complete File Tree: GaDeMa — Feature-Driven Development Structure

```
Gadema.FDD/
│
├── .gitignore                          # Git ignore rules (VS Code + Rider)
├── LICENSE                             # MIT License
├── README.md                           # High-level project overview
├── Directory.Build.props               # Central package management (shared packages)
├── solution.sln                        # Visual Studio Solution file
│
├── docs/                               # Technical documentation
│   ├── SCHEMAv2.md                     # Database schema reference
│   ├── API-CONTRACTSv2.md              # REST endpoint specs
│   ├── CODING_GUIDELINESv2.md          # Naming, testing, security rules
│   ├── FDD_ARCHITECTURE.md             # This document + migration guide
│   └── README.md                       # docs index
│
├── scripts/                            # Build/deploy automation
│   ├── build.ps1                       # Build all features
│   ├── test.ps1                        # Test all features
│   ├── deploy.ps1                      # Deploy to staging/prod
│   └── clean.ps1                       # Clean build outputs
│
├── src/                                # Source code root (all features here)
│   │
│   ├── Features/                       # ⭐ FEATURE-DRIVEN: All features live here
│   │   │
│   │   ├── Gadema.Features.ContentManagement/
│   │   │   ├── Gadema.Features.ContentManagement.csproj          # Feature project file
│   │   │   │
│   │   │   ├── Domain/                           # Pure domain, NO framework deps
│   │   │   │   ├── MetaInfo.cs                # Core entity (Aggregate Root)
│   │   │   │   ├── ContentType.cs               # Domain enum: Character, World...
│   │   │   │   ├── ContentViewMode.cs           # Draft/Review/Published
│   │   │   │   ├── Project.cs                    # Domain model for Projects
│   │   │   │   ├── ProjectTask.cs                # Task entity
│   │   │   │   └── ContentTag.cs                # Tag domain model
│   │   │   │
│   │   │   ├── Application/                      # Business logic & use cases
│   │   │   │   ├── Services/
│   │   │   │   │   ├── IContentService.cs       # Interface contract
│   │   │   │   │   ├── MetaInfoService.cs    # Implementation (business rules only)
│   │   │   │   │   ├── IProjectService.cs
│   │   │   │   │   └── ProjectService.cs
│   │   │   │   └── Commands/                     # CQRS commands
│   │   │   │       ├── CreateMetaInfoCommand.cs
│   │   │   │       ├── UpdateMetaInfoCommand.cs
│   │   │   │       ├── PublishMetaInfoCommand.cs
│   │   │   │       └── RollbackToVersionCommand.cs
│   │   │   │
│   │   │   ├── DomainEvents/                     # Events as first-class citizens
│   │   │   │   ├── MetaInfoUpdatedEvent.cs   # Carries data: old/new values
│   │   │   │   ├── MetaInfoPublishedEvent.cs
│   │   │   │   ├── TaskCompletedEvent.cs        # When a task is marked done
│   │   │   │   └── ProjectCreatedEvent.cs       # Domain event, not application event
│   │   │   │
│   │   │   ├── Infrastructure/                   # Data access & persistence
│   │   │   │   ├── Repositories/
│   │   │   │   │   ├── IRepository{T}.cs        # Generic repository interface
│   │   │   │   │   └── MetaInfoRepository.cs # EF Core-backed implementation
│   │   │   │   ├── Migrations/                   # DB migrations
│   │   │   │   │   ├── 001_initial.sql          # First migration script
│   │   │   │   │   └── 002_add_tags_table.sql  # Subsequent migrations
│   │   │   │   └── MetaInfoEntityTypeConfiguration.cs # Fluent API config
│   │   │   │
│   │   │   ├── Api/                              # HTTP layer (API only)
│   │   │   │   ├── Controllers/
│   │   │   │   │   ├── MetaInfosController.cs    [ApiController] [Route("api/v1/content/items")]
│   │   │   │   │   └── ProjectsController.cs        [ApiController] [Route("api/v1/projects")]
│   │   │   │   └── Models/                         # API-specific DTOs (not in Domain)
│   │   │   │       ├── MetaInfoDto.cs           # Response model
│   │   │   │       └── CreateMetaInfoRequest.cs # Request DTO with validation attributes
│   │   │   │
│   │   │   └── WebApp/                           # Blazor Server UI (feature-specific pages)
│   │   │       ├── Pages/
│   │   │       │   ├── Content.razor              # List view of content items
│   │   │       │   ├── CreateContentDialog.razor  # Modal form for creation
│   │   │       │   └── ViewMetaInfoDialog.razor# Preview/presentation mode
│   │   │       └── Components/
│   │   │           ├── ContentCard.razor          # Reusable card component
│   │   │           └── ContentTypeBadge.razor     # Type badge renderer
│   │   │
│   │   ├── Gadema.Features.ProjectManagement/
│   │   │   ├── Gadema.Features.ProjectManagement.csproj
│   │   │   ├── Domain/
│   │   │   │   ├── Project.cs                     # Aggregate Root with polymorphic ownership
│   │   │   │   ├── Team.cs                        # Team entity (one-to-many: Teams → Members)
│   │   │   │   └── TeamMemberRoleEnum.cs          # Admin/Editor/Viewer roles
│   │   │   ├── Application/
│   │   │   │   ├── Services/IProjectService.cs
│   │   │   │   ├── ProjectService.cs              # Business logic: invite, assign tasks...
│   │   │   │   └── Commands/CreateProjectCommand.cs
│   │   │   ├── Infrastructure/
│   │   │   │   ├── Repositories/TeamRepository.cs
│   │   │   │   └── ProjectEntityTypeConfiguration.cs
│   │   │   └── WebApp/Pages/Projects.razor        # Project dashboard page
│   │   │
│   │   ├── Gadema.Features.DialogueManagement/
│   │   │   ├── Gadema.Features.DialogueManagement.csproj
│   │   │   ├── Domain/
│   │   │   │   ├── DialogueBranch.cs              # Tree node (self-referencing FK)
│   │   │   │   ├── DialogueNode.cs                # Child nodes in branch tree
│   │   │   │   └── BranchEventTypeEnum.cs         # NodeCreated/BranchDeleted events
│   │   │   ├── Application/
│   │   │   │   ├── Services/IDialogueService.cs
│   │   │   │   ├── DialogueService.cs             # Tree traversal, validation
│   │   │   │   └── Commands/AddNodeCommand.cs     # Add child to branch tree
│   │   │   ├── Infrastructure/
│   │   │   │   ├── Repositories/BranchRepository.cs
│   │   │   │   └── DialogueBranchEntityTypeConfiguration.cs
│   │   │   ├── Api/Controllers/DialogueBranchesController.cs
│   │   │   └── WebApp/Pages/Dialogue.razor        # Visual tree builder UI
│   │   │
│   │   ├── Gadema.Features.TaskManagement/
│   │   │   ├── Gadema.Features.TaskManagement.csproj
│   │   │   ├── Domain/ProjectTask.cs              # Flat task structure (no hierarchies)
│   │   │   ├── Application/IProjectTaskService.cs
│   │   │   ├── Infrastructure/TaskRepository.cs   # EF Core with indexes on Status/Priority
│   │   │   └── WebApp/Pages/ProjectTasks.razor    # Kanban-style board
│   │   │
│   │   ├── Gadema.Features.Authentication/
│   │   │   ├── Gadema.Features.Authentication.csproj
│   │   │   ├── Domain/User.cs                      # User entity with password hash
│   │   │   ├── Application/IAuthService.cs         # Sign in, Google OAuth, 2FA
│   │   │   ├── Infrastructure/TokenService.cs      # JWT generation/validation
│   │   │   └── WebApp/Pages/Signin.razor           # Login UI with auth guard
│   │   │
│   │   ├── Gadema.Features.Search/
│   │   │   ├── Gadema.Features.Search.csproj
│   │   │   ├── Domain/SearchQuery.cs               # Query object pattern
│   │   │   ├── Application/ISearchService.cs       # Tantivy full-text search
│   │   │   └── Infrastructure/TantivyIndexProvider.cs  # Index management
│   │   │
│   │   └── Gadema.Features.Export/
│   │       ├── Gadema.Features.Export.csproj
│   │       ├── Domain/ExportFormat.cs               # JSON/PDF/CSV/XML-GDD enum
│   │       ├── Application/IExportService.cs        # Export orchestration
│   │       ├── Infrastructure/UnityExporter.cs      # Unity asset export
│   │       └── Infrastructure/GddXmlExporter.cs     # Unreal GDD XML format
│   │
│   ├── Gadema.Core.Shared/               # ⚠️ SHARED TYPES ONLY (no business logic)
│   │   ├── Gadema.Core.Shared.csproj      <!-- References NO other feature projects -->
│   │   ├── Dtos/Response/SimpleResponseDto.cs  # Generic API response wrapper
│   │   ├── Dtos/PaginationResponse.cs         # Pagination metadata DTO
│   │   └── Enums/SharedEnumExtensions.cs      # Extension methods for enums (shared)
│   │
│   ├── Gadema.Infrastructure.Database/    # ⚠️ SHARED INFRASTRUCTURE ONLY
│   │   ├── Gadema.Infrastructure.Database.csproj <!-- References NO feature projects -->
│   │   ├── GameDbContext.cs                 # Single DbContext with all DbSets
│   │   ├── Migrations/GlobalMigrations/     # Shared migration scripts
│   │   └── EntityConfigurations/Shared/*.cs  # Config for shared entities (User, Team)
│   │
│   └── Gadema.Api.Entry/                  # ⚠️ ENTRY POINT ONLY — NO business logic
│       ├── Gadema.Api.Entry.csproj          <!-- References ALL feature projects -->
│       ├── Program.cs                       # DI registration, middleware pipeline
│       ├── appsettings.json                 # Connection strings, secrets
│       └── appsettings.Development.json     # Dev-only settings
│
└── tests/                                # FEATURE-DRIVEN TESTS (one per feature)
    ├── Gadema.Tests.ContentManagement.csproj
    │   ├── Features/ContentManagement.UnitTests.cs             # Unit tests for domain logic
    │   ├── Application/Services/MetaInfoServiceSpecs.cs     # BDD-style integration tests
    │   └── Api/Controllers/MetaInfosControllerSpecs.cs      # API contract tests
    │
    ├── Gadema.Tests.ProjectManagement.csproj
    │   ├── Features/ProjectManagement.UnitTests.cs
    │   └── Application/Services/ProjectServiceSpecs.cs
    │
    └── Gadema.Tests.IntegrationAll.csproj  # End-to-end API tests against real DB
```

---

## 📋 Key Principles Illustrated in This Structure

| Principle | Implementation | Example |
|-----------|---------------|---------|
| **Feature as a Folder** | Each feature has its own directory with all layers inside | `Features/ContentManagement/Domain/`, `/Application/`, `/Api/`, `/WebApp/` |
| **No Shared Business Logic** | Features never reference each other's implementations | `ContentManagement` does NOT import `ProjectManagement` |
| **Shared Infrastructure is Separate** | DB context, migrations live in a shared project | `Infrastructure.Database/GameDbContext.cs` references NO feature projects |
| **Domain First** | Domain layer has NO dependencies on Application/Infrastructure | `MetaInfo.cs` uses no EF Core or ASP.NET types |
| **CQRS within Features** | Commands and Queries are separate, each with its own handler | `CreateMetaInfoCommandHandler` vs `GetAsync` method |
| **Domain Events as First-Class Citizens** | Events carry data and are published by domain entities | `MetaInfoUpdatedEvent` has `NewTitle`, `OldTitle` properties |
| **Tests Per Feature** | Each feature gets its own test project | `Gadema.Tests.ContentManagement.csproj` lives in `tests/` |

---

## 🔄 Comparison: Layered vs. FDD

```
LAYERED (Before):
┌─────────────────────────────────────────────────┐
│  Gadema.Core        │  All entities mixed       │
│  └── MetaInfo    │  no folder by feature     │
│  └── Project        │                           │
│  └── DialogueBranch │                           │
├─────────────────────────────────────────────────┤
│  Gadema.Api         │  All controllers          │
│  └── Controllers/MetaInfosController.cs      │
│  └── Controllers/ProjectsController.cs          │
├─────────────────────────────────────────────────┤
│  Gadema.WebApp      │  All pages mixed          │
│  └── Pages/Content.razor                        │
│  └── Pages/Projects.razor                       │
└─────────────────────────────────────────────────┘

FDD (After):
┌─────────────────────────────────────────────────┐
│  Features/ContentManagement/                    │
│    ├── Domain/MetaInfo.cs   ← Self-contained │
│    ├── Application/...         │                 │
│    └── WebApp/Pages/Content.razor              │
├─────────────────────────────────────────────────┤
│  Features/ProjectManagement/                    │
│    ├── Domain/Project.cs        ← Self-contained│
│    ├── Application/...          │                 │
│    └── WebApp/Pages/Projects.razor             │
└─────────────────────────────────────────────────┘

Result: Each feature can be understood in isolation.
Swapping out the "Content" feature doesn't touch "Projects".
```

---

## 🚀 How to Add a New Feature (e.g., `Gadema.Features.CharacterManagement`)

1. **Create the project:**
   ```powershell
   dotnet new classlib -n Gadema.Features.CharacterManagement \
     --root-namespace Gadema.Features.CharacterManagement
   ```

2. **Add feature-specific package overrides** in `.csproj`:
   ```xml
   <ItemGroup>
     <PackageReference Include="Blazored.LocalStorage" Version="4.5.0" />
   </ItemGroup>
   ```

3. **Define the domain entity:** `Domain/Character.cs` (no framework deps)

4. **Define application commands:** `Application/Commands/CreateCharacterCommand.cs`

5. **Build the infrastructure layer:** `Infrastructure/Repositories/CharacterRepository.cs`

6. **Add API controller:** `Api/Controllers/CharactersController.cs`

7. **Add Blazor pages:** `WebApp/Pages/Characters.razor`, `WebApp/Components/CharacterCard.razor`

8. **Add tests:** `tests/Gadema.Tests.CharacterManagement.csproj` with BDD specs

**Done.** Zero touching of other features. The new feature can be developed, tested, and deployed independently.