## Complete Project Documentation — GaDeMa v0.1

**GaDeMa (Game Development Management Application)**  
*Version: 0.1 (Pre-Release MVP) | ASP.NET Core 10.0 | .NET 10.0 | SQLite/PostgreSQL | Blazor Server UI*

---

## 📑 Table of Contents

1. [Project Overview](#project-overview)
2. [Architecture & Technology Stack](#architecture--technology-stack)
3. [Database Schema (53 Tables)](#database-schema-53-tables)
4. [Domain Models](#domain-models)
5. [API Endpoints](#api-endpoints)
6. [Entity Framework Configurations](#entity-framework-configurations)
7. [Code Quality Issues](#code-quality-issues)
8. [Documentation Files](#documentation-files)

---

## 📁 Complete File Inventory

### Root Level
| File | Description |
|------|-------------|
| `Gadema.sln` | Visual Studio Solution (Format 12.0, .NET 17/VS2022+) |
| `.gitignore` | VS Code + Rider ignore rules |
| `README.md` | High-level project overview |
| `LICENSE` | MIT License |
| `caddyfile` | Caddy reverse proxy configuration |
| `docker-compose.yml` | Optional local development setup |
| `dotnet-tools.json` | NuGet tools configuration |

### Source Code Structure

```
src/
├── Gadema.Core/                  # Domain Layer (62 files)
│   ├── Dtos/                     # 36 DTOs grouped by domain
│   │   ├── Abilities/           # AbilityDefinition, AbilitySet DTOs
│   │   ├── Attributes/          # CharacterAttributes DTOs
│   │   ├── Authentication/      # SignInDto, GoogleCallbackDto, 2FA DTOs
│   │   ├── Characters/          # CharacterDetails DTOs
│   │   ├── Comments/            # CreateCommentDto, CommentListResponseDto
│   │   ├── ContentItems/        # CreateContentItemDto, UpdateContentItemDto
│   │   ├── DialogueTrees/       # BranchResponseDto, CreateBranchDto
│   │   ├── Export/              # ExportJsonDto, ExportPdfDto, ExportXmlGddDto
│   │   ├── ExternalReferences/  # ReferenceResponseDto
│   │   ├── Narrative/           # SequenceResponseDto
│   │   ├── Projects/            # ProjectResponseDto, CreateProjectDto
│   │   ├── Reviews/             # ApproveContentDto
│   │   ├── Search/              # SearchContentItemsDto
│   │   ├── Tags/                # AddTagsDto, TagListResponseDto
│   │   ├── Tasks/               # TaskResponseDto
│   │   └── Versioning/          # RollbackDto
│   ├── Enums/                    # 19 enums (ContentTypeEnum, TaskStatusEnum, etc.)
│   ├── Models/                   # 31 EF Core entities with full navigation properties
│   └── Services/                 # Interface contracts only (IGademaService, IContentService...)
│
├── Gadema.Data/                  # Persistence Layer (24 files)
│   ├── Configurations/           # Auto-discovered Fluent API configs via ApplyConfigurationsFromAssembly()
│   │   ├── Abilities/           # AbilityDefinitionEntityTypeConfiguration.cs
│   │   ├── Activities/          # ActivityLogEntityTypeConfiguration.cs, TokenUsageLog...
│   │   ├── Attributes/          # AttributeSetEntityTypeConfiguration.cs
│   │   ├── Authentication/      # UserEntityTypeConfiguration.cs, TeamMember...
│   │   │   └── Projects/Narrative/Characters/Attributes/Abilities/Tasks/Versioning/Inventory/Templates/Identity/EngineIntegration/
│   │   ├── Characters/          # CharacterDetailsEntityTypeConfiguration.cs (polymorphic inheritance)
│   │   ├── Content/             # ContentItemEntityTypeConfiguration.cs, DialogueBranch...
│   │   ├── EngineIntegration/   # AssetLinkEntityTypeConfiguration.cs
│   │   ├── Identity/            # ProjectIdentityDefinitionEntityTypeConfiguration.cs
│   │   ├── Inventory/           # EndingDefinitionEntityTypeConfiguration.cs
│   │   ├── Narrative/           # StorySequenceEntityTypeConfiguration.cs, LoreEntry...
│   │   ├── Projects/            # ProjectEntityTypeConfiguration.cs (polymorphic ownership)
│   │   ├── Story/               # StoryBeatEntityTypeConfiguration.cs
│   │   ├── Tasks/               # TaskCommentEntityTypeConfiguration.cs, ReviewStatus...
│   │   ├── Templates/           # TemplateNarrativeStructureEntityTypeConfiguration.cs
│   │   ├── Tokens/              # TokenUsageLogEntityTypeConfiguration.cs
│   │   └── Versioning/          # ContentSnapshotEntityTypeConfiguration.cs (JSON blob storage)
│   └── Database/                 # Single GameDbContext.cs file with DbSet declarations
│       └── GameDbContext.cs      # DbContext with auto-discovered configs + 53 DbSets
│
├── Gadema.Api/                   # Application Layer (42 files)
│   ├── Controllers/              # 12 REST controllers
│   │   ├── Authentication/AuthController.cs                     # POST /api/v1/auth/signin, Google OAuth callback
│   │   ├── Content/CommentsController.cs                        # GET/POST /api/v1/content-items/{id}/comments
│   │   ├── Content/ContentItemsController.cs                    # CRUD + upload/media + autosave/rollback
│   │   ├── Content/DialogueBranchesController.cs                # POST /api/v1/projects/{id}/dialogue/branches
│   │   ├── Content/ExternalReferencesController.cs              # External link management
│   │   ├── Content/ReviewStatusController.cs                    # Review/approve workflow
│   │   ├── Content/SearchController.cs                          # Full-text search (Tantivy)
│   │   ├── Content/TagsController.cs                            # Tag CRUD + Many-to-Many via junction table
│   │   ├── Export/ExportController.cs                           # POST /api/v1/export/{json|pdf|csv|xml-gdd}
│   │   ├── Narrative/StorySequencesController.cs                # Story outline management
│   │   ├── Projects/ProjectsController.cs                       # Project CRUD + token generation
│   │   └── Tasks/ProjectTaskController.cs                       # Flat task structure (ADHD-friendly)
│   ├── Services/                 # 11 service implementations
│   │   ├── Authentication/ApiAuthService.cs                     # JWT, Google OAuth, TOTP 2FA
│   │   ├── Content/ContentItemService.cs                        # Core content CRUD + versioning
│   │   ├── Content/DialogueService.cs                           # Branch tree management
│   │   ├── Content/ExternalReferenceService.cs                  # External link resolver
│   │   ├── Content/TagService.cs                                # Tag resolution service
│   │   ├── EngineIntegration/ExportContentService.cs            # Unity export logic
│   │   ├── EngineIntegration/ExportService.cs                   # PDF generation (QuestPDF), XML GDD export
│   │   ├── Narrative/StoryOutlineService.cs                     # Beat sheet / sequence builder
│   │   ├── Projects/ProjectService.cs                           # Project CRUD + token auth
│   │   ├── Tasks/CommentService.cs                              # Task comment thread management
│   │   └── Tasks/ReviewStatusService.cs                         # Approval workflow engine
│   ├── Program.cs                # DI container, middleware pipeline, health check endpoint
│   └── Services/Interfaces/      # 8 interface contracts (IGademaService, IContentService...)
│
├── Gadema.WebApp/                # Presentation Layer (39 files)
│   ├── Components/Pages/         # 18 Razor pages (Blazor Server)
│   │   ├── Content.razor                    # Content list with filters + CRUD dialog
│   │   ├── Counter.razor                    # [Template page]
│   │   ├── CreateContent.razor              # Form wizard: title → description → publish
│   │   ├── Dialogue.razor                   # Visual tree builder for dialogue nodes
│   │   ├── Error.razor                      # Error boundary component
│   │   ├── ForgotPassword.razor             # Email recovery flow
│   │   ├── Home.razor                       # Landing page with quick actions
│   │   ├── NotFound.razor                   # 404 handler
│   │   ├── ProjectTasks.razor               # Flat task list (priority/difficulty filters)
│   │   ├── Projects.razor                   # Project grid + team member assignment
│   │   ├── Register.razor                   # Registration with optional email verification
│   │   ├── Reviews.razor                    # Review board for team approval workflow
│   │   ├── Search.razor                     # Full-text search across all content types
│   │   ├── Settings.razor                   # Profile, 2FA toggle, recovery codes
│   │   ├── Signin.razor                     # Email/password + Google OAuth button
│   │   ├── Tags.razor                       # Tag browser with auto-complete
│   │   └── Weather.razor                    # [Template page]
│   ├── Components/Layout/         # Shared layout components
│   │   ├── MainLayout.razor             # Full-page layout + SectionOutlet for nav menu
│   │   ├── NavMenu.razor                # Sidebar navigation (projects, content, tasks...)
│   │   ├── ReconnectModal.razor         # Blazor Server reconnection notification
│   │   └── UserMenu.razor               # Dropdown: profile, settings, logout, 2FA toggle
│   ├── Components/Diologues/       # Modal dialogs
│   │   ├── CreateContentDialog.razor    # Modal form for creating content item
│   │   ├── CreateDialogContent.razor    # Multi-step editor inside dialog
│   │   └── EditContentDialog.razor      # In-place editing of existing content
│   ├── App.razor                    # Root component with Routes.razor + SectionOutlet
│   ├── _Imports.razor               # Global using directives (MudBlazor, MudIcons)
│   ├── Program.cs                   # Razor Components + MudBlazor integration
│   └── Properties/launchSettings.json# Local dev server config (port 5001)
│
├── Gadema.Tests/                 # Testing Layer (5 files)
│   ├── Factory/ApiWebApplicationFactory.cs    # Test hosting environment factory
│   ├── Integration/ApiIntegrationTests.cs     # End-to-end API tests with in-memory DB
│   ├── Models/ModelValidationTests.cs         # DTO validation + entity constraints
│   └── Services/ContentItemServiceUnitTest.cs # Unit tests for content service
├── docs/                         # Technical documentation (~60 files)
│   ├── SCHEMAv2.md               # Complete database schema reference
│   ├── API-CONTRACTSv2.md        # REST endpoint specs with request/response examples
│   ├── CODING_GUIDELINESv2.md    # Naming, testing, security rules
│   ├── USER_STORIESv2.md         # Feature breakdown with acceptance criteria
│   ├── WORKFLOWSv2.md            # Single-user & team workflows (UML style text diagrams)
│   ├── SECURITYv2.md             # JWT auth, password hashing, file upload validation
│   ├── PERFORMANCEv2.md          # Caching strategy, query optimization, metrics collection
│   ├── DEPLOYMENTv2.md           # SQLite → PostgreSQL migration guide + CI/CD setup
│   ├── ENTITY_CONFIGURATIONS.md  # Fluent API config reference for all 53 entities
│   ├── Hierarchy.md              # Domain model hierarchy (inheritance diagram)
│   ├── GaDeMa — ADHD-Friendly CMS Design Summ.md    # UX design philosophy document
│   └── docs/deprecated/          # Outdated documentation versions
├── scripts/                      # Build/release automation scripts
└── uploads/                      # Media file storage (gitignored)
```

---

## 🏗️ Architecture & Technology Stack

### Layered Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│   ┌───────────────────────────────────────────────────────┐ │
│   │  Gadema.WebApp (Blazor Server + MudBlazor)           │ │
│   │  • 18 Razor pages for CRUD operations                │ │
│   │  • SectionOutlet-based nav menu                       │ │
│   │  • ReconnectModal for Blazor Server resilience       │ │
│   │  • UserMenu with auth state guard                     │ │
│   └─────────────────────┬─────────────────────────────────┘ │
│                         HTTP (JSON)                          │
├──────────────────────────▼───────────────────────────────────┤
│                      Application Layer                        │
│   ┌───────────────────────────────────────────────────────┐ │
│   │  Gadema.Api (ASP.NET Core Web API v1.0)              │ │
│   │  • REST controllers with [ApiController] + [Route]  │ │
│   │  • Service layer via dependency injection             │ │
│   │  • Swagger/OpenAPI documentation                      │ │
│   └──────────┬─────────────┬──────────────┬──────────────┘ │
│              │             │               │                │
│    ContentItemService    ProjectService    ExportService     │
│    DialogueService       TaskService       AuthService      │
├──────────────┼─────────────┴──────────────┼─────────────────┤
│              ▼                             ▼                  │
│   ┌───────────────────────────────────────────────────────┐ │
│   │  Gadema.Core (Domain Layer)                           │ │
│   │  • Pure C# entities, DTOs, enums — NO framework deps │ │
│   │  • Interface contracts: IGademaService, IContentSvc… │ │
│   └──────────┬─────────────┬──────────────┬──────────────┘ │
│              │             │               │                │
│    IContentService    IProjectService    IExportService     │
├──────────────┼─────────────┴──────────────┼─────────────────┤
│              ▼                             ▼                  │
│   ┌───────────────────────────────────────────────────────┐ │
│   │  Gadema.Data (Persistence Layer)                      │ │
│   │  • GameDbContext with Fluent API configs             │ │
│   │  • Auto-discovery via ApplyConfigurationsFromAssembly()│ │
│   │  • SQLite (MVP) / PostgreSQL (Production)            │ │
│   └─────────────────────────────┬─────────────────────────┘ │
│                                 │                            │
├──────────────────────────────────▼──────────────────────────┤
│                         Database Layer                        │
│              • 53 tables with FK constraints                   │
│              • Polymorphic inheritance (CharacterDetails)     │
│              • JSON blob storage for snapshots                 │
└─────────────────────────────────────────────────────────────┘
```

### Technology Choices Rationale

| Concern | Choice | Why |
|---------|--------|-----|
| **Framework** | ASP.NET Core 10.0 Web API + Blazor Server | Full .NET ecosystem, rapid UI development with MudBlazor |
| **UI Library** | MudBlazor v6.19.0 | Bootstrap 5-based, Material Design components, minimal CSS bloat |
| **ORM** | EF Core 10.0 | Auto-mapping DTOs ↔ Entities, Fluent API for migrations |
| **Database** | SQLite (MVP) → PostgreSQL (Prod) | Zero-config local dev; horizontal scaling in production |
| **PDF Export** | QuestPDF v2026.7.3 | Declarative PDF generation with watermarking support |
| **Validation** | FluentValidation v11.9.0 | Type-safe validation rules, reusable validators |
| **Testing** | xUnit + FluentAssertions + Moq | Familiar syntax, expressive assertions, mockable services |

### Build Configuration (`Directory.Build.props`)

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    <TargetFramework>net10.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
  </PropertyGroup>

  <!-- Central Package References (all versions defined here) -->
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.11" />
    <PackageReference Include="QuestPDF" Version="2026.7.3" />
    <PackageReference Include="MudBlazor" Version="6.19.0" />
    <!-- ... 14 more packages -->
  </ItemGroup>
</Project>
```

---

## 🗄️ Database Schema (53 Tables)

### Authentication & Team Management

| Table | Primary Key | Foreign Keys | Description |
|-------|------------|--------------|-------------|
| `Users` | `Id` (Guid) | — | User accounts with BCrypt/RFC2898DeriveBytes hashed passwords |
| `Teams` | `Id` (Guid) | `OwnerId` → Users.Id | Collaborative project groups |
| `TeamMembers` | `Id` (Guid) | `TeamId`, `UserId` | Many-to-many with role enum |
| `ProjectTokens` | `Id` (Guid) | `ProjectId` → Projects.Id | API tokens per project (SHA256 hashed secrets) |
| `TokenUsageLogs` | `Id` (Guid) | `ProjectTokenId` → ProjectTokens.Id | Audit trail of token usage |
| `ActivityLogs` | `Id` (Guid) | `ProjectId`, `ContentItemId` | Immutable audit trail of all DB changes |

### Core Content Management

| Table | Primary Key | Foreign Keys | Description |
|-------|------------|--------------|-------------|
| `Projects` | `Id` (Guid) | `OwnerTypeId` → Enum; `SeriesProjectId` → Projects.Id | Game projects with polym