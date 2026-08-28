
---

## 🐱 GaDeMa – Game Development Management Application  
## **Version 0.1 | Pre-Release Preview**  

![Build Status](https://github.com/yourusername/GaDeMa/actions/workflows/ci.yml/badge.svg) ![License](https://img.shields.io/badge/license-MIT-blue.svg) ![.NET Version](https://img.shields.io/badge/.NET-10.0.11-orange.svg)

---

## **🎯 Purpose & Vision**

GaDeMa is a **collaborative documentation platform for game development, narrative design, and project management**. It supports structured content management, team collaboration, version control with rollback, AI-generated content integration, and export to GDD formats compatible with Unity/Unreal Engine.

### **Core Philosophy**
- 🎨 **Visual Power**: Professional-grade outlines, hierarchical dialogue trees, ability systems  
- 🧠 **ADHD-Friendly**: Flat structure, quick wins, focus mode for writers with focus challenges  
- 🌐 **Dual-Purpose**: Private writing + public presentation capabilities  
- 💼 **Professional Standards**: Industry-standard features for game developers (Unreal/Unity integration)  

### **Target Users**
- Novice/Amateur Writers (simple workflow)  
- Professional Novelists (visual outlining, beat sheets)  
- Visual Novel Developers (branching narrative support)  
- ADHD Writers (focus tools, task simplification)  
- Game Developers (Unreal/Unity integration)

---

## **🛠️ Technology Stack**

| Component | Specification |
| :--- | :--- |
| **Framework** | ASP.NET Core 10.0.11 Web API + Blazor Server |
| **Database** | SQLite (file-based for MVP; Production-ready for PostgreSQL) |
| **Hosting** | Caddy reverse proxy (HTTP/2, SSL auto-renewal) |
| **Authentication** | Google OAuth + Password Login + 2FA Support |
| **Export Formats** | JSON + PDF + CSV (Unity) + XML GDD (Unreal) |

---

## **🚀 Getting Started**

### **Prerequisites**
- **.NET 10.0 SDK** ([download](https://dotnet.microsoft.com/download))
- **SQLite CLI** (for local development)
- **Git** (version control)

### **Quick Start**
```bash
# 1. Clone the repository
git clone https://github.com/yourusername/GaDeMa.git
cd GaDeMa

# 2. Restore dependencies
dotnet restore

# 3. Run migrations (creates database)
dotnet ef database update

# 4. Build and run API locally
dotnet run --project src/Gadema.Api

# 5. Run Blazor WebApp locally
dotnet run --project src/Gadema.WebApp
```

---

## **✨ Key Features**

- 📖 **Visual Outlining**: Professional-grade chapter/outline structure for novelists (Option B)
- 🎭 **Hierarchical Dialogue Trees**: Branching narrative paths for visual novels (Twine/Ink compatible)
- 🧠 **ADHD-Friendly Tasks**: Flat structure with quick wins and focus mode support
- 🌐 **Public/Private Views**: Clean presentation layout separate from admin editing
- 🔗 **External References**: Structured links to Google Docs, Pinterest boards, Figma files
- 🎮 **Engine Integration**: Export formats compatible with Unity CSV/XML import + Unreal XML GDD

---

## **📚 Documentation**

For technical details about the architecture, schema, API endpoints, and implementation guidelines:

| Document | Description |
| :--- | :--- |
| **Database Schema** | `/docs/SCHEMA.md` (~53 tables) |
| **API Contracts** | `/docs/API-CONTRACTS.md` (Endpoints & DTOs) |
| **Coding Guidelines** | `/docs/CODING_GUIDELINES.md` (Naming, testing standards) |
| **User Stories** | `/docs/USER_STORIES.md` (Feature breakdown) |
| **Workflow Examples** | `/docs/WORKFLOWS.md` (Single user, team use cases) |

---

## **🔐 Security & Configuration**

- ✅ Global feature flags (`EnableUserRegistration = false` by default in production)
- ✅ API token management with SHA256 hashing for automation (CI/CD)
- ✅ DDoS protection via Caddy rate limiting
- ✅ 2FA support for sensitive projects

---

## **🖥️ Deployment Options**

| Platform | Description |
| :--- | :--- |
| **Web App** | Blazor Server hosted on VPS (10+ GB RAM, 8+ CPU cores) |
| **Desktop Client** | .NET MAUI or Avalonia UI (future phase) |
| **Mobile App** | .NET MAUI for iOS/Android (future phase) |

---

## **📦 Export Formats**

| Format | Use Case | Engine Support |
| :--- | :--- | :--- |
| **JSON** | General purpose, version tracking | ✅ |
| **PDF** | Publication-ready GDD documents | ✅ |
| **CSV** | Unity import (character sheets) | ✅ |
| **XML** | Unreal Engine GDD integration | ✅ |

*All exports support selective section filtering and optional watermarking for IP protection.*

---

## **🔗 Support & Feedback**

- **GitHub Issues**: [yourusername/GaDeMa/issues](https://github.com/yourusername/GaDeMa/issues)
- **Email Support**: admin@example.com
- **Documentation**: See `/docs/` folder for technical guides

---

**Built with ❤️ for game developers, narrative designers, and indie studios.**  
**GaDeMa v0.1 | Pre-Release Preview (Production-Ready Architecture)**