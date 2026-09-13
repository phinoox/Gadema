# 🐱 GaDeMa – Game Development Management Application

**Version**: v0.1 (Pre-Release MVP)  
**Status**: Production-Ready Architecture  
**Last Updated**: 2024  

---

## 📋 Overview

GaDeMa is a **collaborative documentation platform for game development, narrative design, and project management**. It supports structured content management, team collaboration, version control with rollback, AI-generated content integration, and export to GDD formats compatible with Unity/Unreal Engine.

### 🎯 Core Philosophy
- 🎨 **Visual Power**: Professional-grade outlines, hierarchical dialogue trees, ability systems  
- 🧠 **ADHD-Friendly**: Flat structure, quick wins, focus mode for writers with focus challenges  
- 🌐 **Dual-Purpose**: Private writing + public presentation capabilities  
- 💼 **Professional Standards**: Industry-standard features for game developers (Unreal/Unity integration)

### 👥 Target Users
- Novice/Amateur Writers (simple workflow)  
- Professional Novelists (visual outlining, beat sheets)  
- Visual Novel Developers (branching narrative support)  
- ADHD Writers (focus tools, task simplification)  
- Game Developers (Unreal/Unity integration)

---

## 🛠️ Technology Stack

| Component | Specification |
| :--- | :--- |
| **Framework** | ASP.NET Core 10.0.11 Web API + Blazor Server |
| **Database** | SQLite (file-based for MVP; Production-ready for PostgreSQL migration) |
| **Hosting** | Caddy reverse proxy (HTTP/2, SSL auto-renewal) |
| **Authentication** | Google OAuth + Password Login + 2FA Support |
| **Export Formats** | JSON + PDF (QuestPDF) + CSV (Unity) + XML GDD (Unreal) |
| **Testing** | xUnit + FluentAssertions (≥80% code coverage) |

---

## 📁 Project Structure

```bash
GaDeMa/
├── .gitignore                        # Git ignore rules (VS Code + Rider support)
├── LICENSE                           # MIT License file
├── README.md                         # High-level project overview
├── AppHost.csproj                    # Solution file (root project reference)
├── caddyfile                         # Caddy reverse proxy configuration
├── docker-compose.yml                # Optional local development setup
├── dotnet-tools.json                 # NuGet tools configuration
├── docs/                             # Technical documentation
│   ├── SCHEMA.md                     # Database schema definitions (~53 tables)
│   ├── API-CONTRACTS.md              # REST endpoint specifications
│   ├── CODING_GUIDELINES.md          # Naming, testing, security rules
│   ├── USER_STORIES.md               # Feature breakdown & acceptance criteria
│   ├── WORKFLOWS.md                  # Single user & team use cases
│   ├── SECURITY.md                   # Authentication, authorization, file upload
│   ├── PERFORMANCE.md                # Caching, query optimization, metrics
│   ├── DEPLOYMENT.md                 # Database migration, hosting, CI/CD
│   └── SUMMARY.md                    # Quick reference guide
├── src/                              # Source code root
│   ├── Gadema.Core/                 # Shared domain models & interfaces
│   │   ├── Models/                   # EF Core entities (e.g., MetaInfo.cs)
│   │   ├── Dtos/                     # API DTOs (e.g., CreateProjectDto.cs)
│   │   ├── Enums/                    # Type enumerations
│   │   └── Interfaces/               # Service contracts (e.g., IContentService.cs)
│   ├── Gadema.Data/                 # EF Core DbContext + Migrations config
│   ├── Gadema.Api/                  # ASP.NET Core Web API layer
│   │   ├── Controllers/              # REST endpoints (e.g., MetaInfoController.cs)
│   │   ├── Services/                 # Business logic (e.g., MetaInfoService.cs)
│   │   ├── Middleware/               # Auth policies, rate limiting
│   │   └── Program.cs                # Application entry point + DI setup
│   ├── Gadema.WebApp/               # Blazor Server App layer
│   │   ├── Pages/                    # Razor pages (e.g., Index.razor)
│   │   ├── Components/               # Reusable UI components
│   │   └── Layouts/                  # Main layout with auth guard
│   └── Gadema.Tests/                # Unit + Integration tests
├── uploads/                          # Media files (outside src/ for git ignore)
└── docs/                             # Technical documentation
```

---

## 🚀 Getting Started

### Prerequisites
- .NET 10.0 SDK or later
- Git
- Visual Studio Code or JetBrains Rider

### Local Development Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd GaDeMa
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the API (local development)**
   ```bash
   cd src/Gadema.Api
   dotnet run
   ```

4. **Run the Web App (Blazor Server)**
   ```bash
   cd src/Gadema.WebApp
   dotnet run
   ```

### Database Setup

**Local Development**: SQLite (file-based)  
**Production**: PostgreSQL (service-based)

See `docs/DEPLOYMENT.md` for database migration details.

---

## 📚 Documentation

| Document | Description |
| :--- | :--- |
| **SCHEMA.md** | Complete database schema (~53 tables) with Fluent API configuration |
| **API-CONTRACTS.md** | REST endpoint specifications with request/response examples |
| **CODING_GUIDELINES.md** | Naming, testing, security rules for implementation |
| **USER_STORIES.md** | Feature breakdown & acceptance criteria |
| **WORKFLOWS.md** | Single user & team use cases |
| **SECURITY.md** | Authentication, authorization, file upload security |
| **PERFORMANCE.md** | Caching, query optimization, metrics |
| **DEPLOYMENT.md** | Database migration, hosting, CI/CD setup |
| **SUMMARY.md** | Quick reference guide |

---

## 🎯 Key Features (MVP)

### Authentication & Security
- ✅ Google OAuth + Password Login
- ✅ Two-Factor Authentication (TOTP)
- ✅ API Token Management (SHA256 hashed)
- ✅ Global Feature Flags (`EnableUserRegistration`)

### Content Management
- ✅ Structured content with view mode separation
- ✅ Version control with rollback support
- ✅ External reference management (Google Docs, Pinterest)
- ✅ Media file upload (max 100MB per file)

### Project Structure
- ✅ Flat task structure (ADHD-friendly)
- ✅ Story outlining with sequences/sequences
- ✅ Dialogue tree support (branching narratives)
- ✅ Ability systems (GAS-like architecture)

### Export & Integration
- ✅ JSON export for Unity/Unreal
- ✅ PDF export with watermarking
- ✅ CSV export for Unity character sheets
- ✅ XML GDD export for Unreal Engine

---

## 🧠 ADHD-Friendly Design

- **Flat Task Structure**: No hierarchical epics/stories
- **Quick Wins**: Prioritize tasks with `Difficulty = Easy` or `IsQuickWin = true`
- **Focus Mode**: Single-task views that reduce cognitive load
- **View Mode Separation**: Clean admin vs. public views

---

## 🛡️ Security Features

- ✅ Password hashing (BCrypt/RFC2898DeriveBytes)
- ✅ API token SHA256 hashing with unique salts
- ✅ File upload validation (100MB max, MIME type checking)
- ✅ JWT token creation/validation
- ✅ Input sanitization (HTML escaping to prevent XSS)
- ✅ DDoS protection (Caddy rate limiting: 100 req/min per IP)

---

## 📊 Performance Guidelines

- ✅ Always use `.Include()` for eager loading (prevent N+1 queries)
- ✅ Paginate list endpoints using `Skip()`/`Take()` (default page size: 20)
- ✅ Configure indexes in `OnModelCreating()` for frequently filtered columns
- ✅ Redis caching strategy for frequently accessed data

---

## 🧪 Testing Requirements

- ✅ Unit tests with ≥80% code coverage
- ✅ Integration tests for API endpoints
- ✅ Test edge cases (null inputs, empty collections)
- ✅ Descriptive test method names

---

## 📝 Git Commit Guidelines

```bash
git commit -m "<Type>: <Subject>"
# Examples:
git commit -m "feat: add external reference support"
git commit -m "fix: resolve pagination issue in task list"
git commit -m "docs: update README with v0.1 features"
```

### Branch Naming Convention
| Branch Type | Pattern | Example |
| :--- | :--- | :--- |
| Feature | `feature/{name}` | `feature/external-references` |
| Bug Fix | `fix/{bug-description}` | `fix/view-mode-separation-bug` |
| Documentation | `docs/{topic}` | `docs/update-gitignore-rules` |
| Chore | `chore/{task}` | `chore/update-nuget-packages` |

---

## 🚀 Deployment Strategy

### Database Migration Path (SQLite → PostgreSQL)
| Environment | Database Provider | Rationale |
| :--- | :--- | :--- |
| **Local Development** | SQLite (file-based) | Fast, no service required |
| **MVP Launch** | SQLite (file-based) | Minimal hosting overhead |
| **Production** | PostgreSQL (service) | Scalability, multi-user concurrency |

### Deployment Options
- **Web App**: Blazor Server hosted on VPS (10+ GB RAM, 8+ CPU cores)
- **Desktop Client**: .NET MAUI or Avalonia UI (future phase)
- **Mobile App**: .NET MAUI for iOS/Android (future phase)

---


## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- Built with ❤️ for game developers and writers
- Inspired by industry-standard GDD tools (Unreal/Unity)
- Designed with ADHD-friendly principles in mind

---

**Version**: v0.1 (Pre-Release MVP)  
**Status**: prototyping ✅  
**Last Updated**: 2026  

🐱 **GaDeMa Team** 🛠️