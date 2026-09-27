# 📄 **API-CONTRACTS.md** – Updated with Domain Clustering & Response Patterns
**GaDeMa – Game Development Management Application (v0.1 Pre-Release MVP)**  
**Status**: Complete with Project Model ✅, Hybrid Response Patterns ⭐, and Domain-Aware Endpoints  

---

## **📋 Overview**

This document defines the complete API contract for GaDeMa v0.1, including:
- ✅ All REST endpoint specifications (Authentication, Projects, Content, Tasks, Export)
- ✅ Request/response formats with examples (hybrid pattern: wrapped vs. raw responses)
- ✅ Authentication and authorization requirements
- ✅ Error handling patterns
- ✅ DTO design patterns and validation rules
- ✅ Pagination conventions
- ✅ View mode separation logic

**Total Endpoints**: ~35+ | **Framework**: ASP.NET Core Web API + Blazor Server  
**Base URL**: `/api/v1/`  

---

## **📁 Updated File Structure Reference**

```bash
src/
├── Gadema.Core/Models/          # Entity classes (clustered by domain)
│   ├── Authentication/User.cs
│   ├── Projects/Project.cs
│   ├── Content/MetaInfo.cs
│   ├── Tasks/ProjectTask.cs      # Renamed from Task to avoid System.Threading.Task ambiguity
│   └── [etc...]
├── Gadema.Core/Dtos/            # API DTOs (clustered by domain)
│   ├── Authentication/SigninDto.cs
│   ├── Projects/CreateProjectDto.cs
│   ├── Content/MetaInfoCreateDto.cs
│   ├── Tasks/ProjectTaskCreateDto.cs  # Renamed from TaskCreateDto
│   └── [etc...]
├── Gadema.Api/Controllers/    # REST endpoints (clustered by domain)
│   ├── Authentication/AuthController.cs
│   ├── Projects/ProjectsController.cs
│   ├── Content/MetaInfosController.cs
│   ├── Tasks/TasksController.cs      # Renamed from TaskController
│   └── [etc...]
```

---

## **🔐 1. Base URL Structure** (Unchanged)

```
Production: https://gaema-api.com/api/v1/
Staging:    https://staging.gaema-api.com/api/v1/
Local Dev:  http://localhost:5000/api/v1/
```

All endpoints follow the v1 versioning convention: `/api/v1/{resource}`

---

## **🔐 2. Authentication & Authorization** (Unchanged)

### **Authentication Methods**

| Endpoint Type | Auth Required | Method | Notes |
| :--- | :--- | :--- | :--- |
| Public Endpoints (Auth) | ✅ Yes | OAuth2, Password, or API Token | No bearer token needed |
| User Management | ✅ Yes | Bearer Token (User Auth) | JWT or session token |
| Project Operations | ✅ Yes | Bearer Token (Project Owner/Member) | Check role-based permissions |
| Export Operations | ⚠️ Optional | API Token for automation (CI/CD) | Use hashed project token hash |

### **JWT Token Usage**
```bash
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### **API Token Usage** (for CI/CD pipelines)
```bash
Authorization: Bearer {token-hash-from-ProjectToken}
```

---

## **🔐 3. Error Response Format** (Unchanged - Hybrid Pattern Added)

All endpoints use standard error response format for 4xx status codes:

### **Standard Error Response (4xx)**
```json
{
  "success": false,
  "errors": ["Description cannot be empty", "Title is required"]
}
```

### **Not Found Error (404)**
```json
{
  "success": false,
  "message": "Resource not found"
}
```

### **Unauthorized Error (401)**
```json
{
  "success": false,
  "errors": ["Authentication required"],
  "message": "Invalid or missing authentication token"
}
```

---

## **🔐 4. Pagination Pattern** (Unchanged)

All list endpoints use standard pagination:

### **Pagination Query Parameters**
| Parameter | Type | Default | Notes |
| :--- | :--- | :--- | :--- |
| `page` | int | 1 | Page number (1-based) |
| `pageSize` | int | 20 | Items per page (1-100) |

### **Pagination Response Structure**
```json
{
  "data": [...],
  "pagination": {
    "currentPage": 1,
    "pageSize": 20,
    "totalItems": 100,
    "totalPages": 5
  }
}
```

---

## **🔐 5. View Mode Separation** (Unchanged)

API endpoints support both `PrivateWriting` and `Presentation` view modes for MetaInfo responses:

### **View Mode Query Parameter**
| Parameter | Type | Default | Notes |
| :--- | :--- | :--- | :--- |
| `viewMode` | enum | PrivateWriting | PrivateWriting or Presentation |

### **Response Differences**

**PrivateWriting Mode**: Full admin interface (includes all fields + version history)

**Presentation Mode**: Clean public view (only published fields, no admin details)

```csharp
// In controller action parameter:
[FromQuery] public ViewModeEnum ViewMode { get; set; } = ViewModeEnum.PrivateWriting;
```

---

## **🔐 6. DTO Design Patterns** (Updated with Domain Clustering)

### **Naming Convention Rules**
- ✅ `CreateXDto` for creation operations (e.g., `CreateProjectDto`, `CreateMetaInfoDto`)
- ✅ `UpdateXDto` for partial updates (e.g., `UpdateMetaInfoDto`, `UpdateProjectTaskDto`)
- ✅ `ResponseXDto` for API responses (e.g., `MetaInfoResponseDto`, `ProjectTaskResponseDto`)
- ✅ All DTOs must be marked with `[Display(Name = "...")]` for UI labels

### **Required vs Optional Field Patterns**
```csharp
// Required fields use [Required] attribute
[Required] public string Title { get; set; } = "";

// Optional fields use nullable types or default values
public string? Description { get; set; } = null!;
public int Version { get; set; } = 0;

// Collections should be initialized with empty list (not null)
public ICollection<Guid> TagIds { get; set; } = new List<Guid>();
```

### **Validation Attributes Placement**
- ✅ Place validation attributes on DTO properties, NOT models
- ✅ Use `[MaxLength]` for string fields in DTOs (e.g., 128 chars)
- ✅ Use `[Range]` for numeric fields (e.g., page size 1-100)

---

## **🔐 7. Authentication Endpoints** (Unchanged - Examples Updated with Hybrid Pattern)

### **POST /api/v1/auth/signin** - Traditional Login
**Description**: Authenticate user with email/password (if registration enabled globally).

| Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `email` | string | ✅ Yes | Email or Google Subject ID |
| `password` | string | ❌ No | For traditional login only |
| `twoFactorToken` | string | ❌ No | TOTP token for 2FA (optional) |

**Request Body**:
```json
{
  "email": "user@example.com",
  "password": "secure_password"
}
```

**Response** ✅ **WRAPPED** - User-triggered confirmation:
```json
{
  "success": true,
  "message": "Authentication successful. Welcome back!",
  "data": {
    "tokenType": "Bearer",
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresInSeconds": 3600,
    "refreshToken": "dGVzdC1yZWZyZXNoLXRva2Vu",
    "user": {
      "id": "usr-001",
      "name": "Jane Doe",
      "email": "jane@example.com"
    }
  }
}
```

---

### **GET /api/v1/auth/recovery-codes** - View Recovery Codes
**Description**: Retrieve recovery codes for current user account.

| Query Param | Type | Notes |
| :--- | :--- | :--- |
| `format` | string | "csv" (default) or "text" |

**Response** ✅ **RAW** - Simple data retrieval:
```json
{
  "recoveryCodes": [
    "JBSWY3DPEHPK3PXP",
    "HBB6Z4M5VX2G9K8L",
    "PQR7T9U1W2X3Y4Z5"
  ],
  "usageInstructions": "Use one of these codes if you lose access to your authenticator app."
}
```

---

### **POST /api/v1/auth/2fa/disable** - Disable 2FA
**Description**: Disable two-factor authentication for current user account.

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `twoFactorToken` | string | ✅ Yes | Current TOTP token for verification |

**Response** ✅ **WRAPPED** - User-triggered confirmation:
```json
{
  "success": true,
  "message": "2FA has been disabled successfully",
  "data": null
}
```

---

## **🔐 8. Projects Endpoints** (Updated with Project Model)

### **GET /api/v1/projects** - List Projects
**Description**: Get a list of all projects (filtered by user/team ownership).

| Query Param | Type | Default | Notes |
| :--- | :--- | :--- | :--- |
| `page` | int | 1 | Page number |
| `pageSize` | int | 20 | Items per page |
| `status` | enum | All | Draft, InProgress, Published, Archived |
| `visibility` | enum | All | Public/Private |
| `search` | string | - | Search by title/slug |

**Response** ✅ **RAW** - Simple list retrieval:
```json
{
  "data": [
    {
      "id": "proj-001",
      "title": "The Dragon's Crown",
      "slug": "the-dragons-crown",
      "ownerType": 0,  // User (0) or Team (1)
      "ownerId": "usr-001",
      "seriesId": null,
      "visibility": 2,  // Public (2) or Private (1)
      "status": 1,     // InProgress
      "enableUserRegistration": false,
      "allowManualInvites": true,
      "viewMode": "PrivateWriting",
      "createdAt": "2024-03-15T10:00:00Z"
    }
  ],
  "pagination": {
    "currentPage": 1,
    "totalItems": 10,
    "totalPages": 1
  }
}
```

---

### **POST /api/v1/projects** - Create Project
**Description**: Create new project (user/team owned + template selection).

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `title` | string | ✅ Yes | Project title |
| `slug` | string | ❌ No | Optional, auto-generated if not provided |
| `templateId` | Guid | ❌ No | Use project template for structure |
| `visibility` | int | Default 1 | Private (1) or Public (2) |

**Request Body** ✅ **RAW**:
```json
{
  "title": "The Dragon's Crown",
  "slug": null,
  "templateId": null,
  "visibility": 2
}
```

**Response** ✅ **RAW** - Simple creation response:
```json
{
  "id": "proj-001",
  "title": "The Dragon's Crown",
  "slug": "the-dragons-crown",
  "ownerType": 0,
  "enableUserRegistration": false,
  "allowManualInvites": true,
  "viewMode": "PrivateWriting",
  "createdAt": "2024-03-15T10:00:00Z"
}
```

---

### **PUT /api/v1/projects/{id}** - Update Project
**Description**: Update project ownership, visibility, publish, transfer ownership.

| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `visibility` | int | ❌ No | 1 (Private) or 2 (Public) |
| `enableUserRegistration` | bool | ❌ No | Enable self-registration |
| `allowManualInvites` | bool | ❌ No | Allow team invitations |

**Request Body** ✅ **RAW**:
```json
{
  "visibility": 2,
  "enableUserRegistration": true,
  "allowManualInvites": false
}
```

**Response** ✅ **WRAPPED** - Confirmation:
```json
{
  "success": true,
  "message": "Project settings updated successfully",
  "data": {
    "id": "proj-001",
    "title": "The Dragon's Crown",
    "visibility": 2,  // Updated to Public
    "enableUserRegistration": true,  // Now enabled
    "lastModified": "2024-03-15T12:00:00Z"
  }
}
```

---

### **DELETE /api/v1/projects/{id}** - Transfer/Delete Project
**Description**: Transfer or delete project.

| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |

**Response** ✅ **WRAPPED** - Confirmation:
```json
{
  "success": true,
  "message": "Project has been transferred to: [username]",
  "data": null
}
```

---

### **POST /api/v1/projects/{id}/tokens** - Create API Token
**Description**: Create a project-level API token for automation access.

| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `tokenName` | string | ✅ Yes | e.g., "CI/CD Pipeline" |
| `permissions` | array[string] | ❌ No | ["Export", "Read", "Publish"] |
| `expiresAt` | string | ❌ No | ISO 8601 date/time (optional) |

**Request Body** ✅ **RAW**:
```json
{
  "tokenName": "CI/CD Pipeline",
  "permissions": ["Export", "Read"],
  "expiresAt": null
}
```

**Response** ✅ **WRAPPED** - Confirmation:
```json
{
  "success": true,
  "message": "API token created successfully",
  "data": {
    "id": "tok-001",
    "name": "CI/CD Pipeline",
    "hash": "dG9rZW4tY2hhcy0xMjM=",  // Encoded hash (not plain text)
    "isActive": true,
    "expiresAt": null,
    "permissions": ["Export", "Read"],
    "createdAt": "2024-03-15T14:30:00Z"
  }
}
```

---

### **GET /api/v1/projects/{id}/tokens** - List Project Tokens
**Description**: List all API tokens for a project.

| Query Param | Type | Default | Notes |
| :--- | :--- | :--- | :--- |
| `includeUsage` | bool | false | Include usage logs |

**Response** ✅ **RAW**:
```json
{
  "tokens": [
    {
      "id": "tok-001",
      "name": "CI/CD Pipeline",
      "isActive": true,
      "lastUsedAt": "2024-03-15T14:30:00Z",
      "usageCount": 15,
      "ipAddress": "192.168.1.100"
    }
  ]
}
```

---

### **DELETE /api/v1/projects/{id}/tokens/{tokenId}** - Revoke API Token
**Description**: Revoke or rotate an existing API token.

| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |
| `tokenId` | Guid | Token ID to revoke |

**Response** ✅ **WRAPPED** - Confirmation:
```json
{
  "success": true,
  "message": "Token has been revoked successfully",
  "data": null
}
```

---

## **🔐 9. Content Items Endpoints** (Updated with Hybrid Response Pattern)

### **GET /api/v1/content/items** - List Content Items
**Description**: List content for project (filtered by published, status, tags).

| Path/Query Param | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `projectId` | Guid | ❌ No | Filter by project (required in practice) |
| `contentType` | enum | ❌ No | Filter by content type |
| `status` | enum | ❌ No | Filter by status |
| `published` | bool | ❌ No | Default: true (for public view) |
| `viewMode` | enum | ❌ No | PrivateWriting or Presentation |

**Response** ✅ **RAW** - Simple list retrieval:
```json
{
  "data": [
    {
      "id": "char-001",
      "title": "Geralt of Rivia",
      "slug": "geralt-of-rivia",
      "contentType": 1,
      "description": "...",
      "published": true,
      "viewMode": "PrivateWriting",
      "version": 3
    }
  ],
  "pagination": {
    "currentPage": 1,
    "totalItems": 25,
    "totalPages": 3
  }
}
```

---

### **GET /api/v1/content/items/{id}** - Get Content Item by ID
**Description**: Get full content item with media, attributes, background.

| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

| Query Param | Type | Default | Notes |
| :--- | :--- | :--- | :--- |
| `viewMode` | enum | PrivateWriting | PrivateWriting or Presentation |
| `includeReferences` | bool | false | Include external references if true |

**Response** ✅ **RAW** - Simple data retrieval:
```json
{
  "id": "char-001",
  "title": "Geralt of Rivia",
  "slug": "geralt-of-rivia",
  "contentType": 1,
  "description": "...",
  "published": true,
  "viewMode": "Presentation",  // Clean public view
  "version": 3,
  "externalReferences": [
    {
      "url": "https://notion.so/game-team/gdd",
      "title": "Official GDD Document"
    }
  ]
}
```

---

### **POST /api/v1/content/items** - Create/Edit Content Item
**Description**: Create content item with dedicated outline first.

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `projectId` | Guid | ✅ Yes | Project ID |
| `contentType` | enum | ✅ Yes | e.g., Character, World, Mechanics |
| `title` | string | ✅ Yes | Content item title |
| `slug` | string | ❌ No | Optional, auto-generated if not provided |
| `description` | string | ❌ No | Full description (Markdown/HTML) |
| `shortDesc` | string | ❌ No | Summary for search/filtering |

**Request Body** ✅ **RAW**:
```json
{
  "projectId": "proj-001",
  "contentType": "Character",
  "title": "Geralt of Rivia",
  "slug": null,
  "description": "...",
  "shortDesc": "Main protagonist character"
}
```

**Response** ✅ **RAW** - Simple creation response:
```json
{
  "id": "char-001",
  "title": "Geralt of Rivia",
  "slug": "geralt-of-rivia",
  "contentType": 1,
  "published": false,  // Default to draft
  "viewMode": "PrivateWriting",
  "version": 1,
  "status": "Draft"
}
```

---

### **PUT /api/v1/content/items/{id}** - Update Content Item
**Description**: Update content item (description, publish status, view mode).

| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `description` | string | ❌ No | Updated description |
| `published` | bool | ❌ No | Publish/unpublish |
| `viewMode` | enum | ❌ No | Switch between PrivateWriting and Presentation |

**Request Body** ✅ **RAW**:
```json
{
  "description": "...updated...",
  "published": true,
  "viewMode": "Presentation"
}
```

**Response** ✅ **WRAPPED** - Confirmation:
```json
{
  "success": true,
  "message": "Content item updated successfully",
  "data": {
    "id": "char-001",
    "title": "Geralt of Rivia",
    "description": "...updated...",
    "published": true,
    "viewMode": "Presentation",
    "version": 2,
    "status": "Published"
  }
}
```

---

### **DELETE /api/v1/content/items/{id}** - Delete Content Item (Admin Only)
**Description**: Delete content item (admin only).

| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

**Response** ✅ **WRAPPED** - Confirmation:
```json
{
  "success": true,
  "message": "Content item has been deleted successfully",
  "data": null
}
```

---

### **POST /api/v1/content-items/{id}/upload** - Upload Media File
**Description**: Upload file to media area.

| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

| Form Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `file` | binary | ✅ Yes | Media file to upload (max 100MB) |

**Response** ✅ **WRAPPED** - Confirmation:
```json
{
  "success": true,
  "message": "Media file uploaded successfully",
  "data": {
    "id": "att-001",
    "filename": "character-concept.png",
    "contentType": "image/png",
    "storagePath": "/uploads/2024/03/char-concept.png",
    "uploadedAt": "2024-03-15T14:30:00Z"
  }
}
```

---

### **POST /api/v1/content-items/{id}/autosave** - Auto-Save Snapshot
**Description**: Auto-save snapshot for versioning.

| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

**Response** ✅ **RAW** - Simple data retrieval:
```json
{
  "snapshotId": "snap-001",
  "version": 2,
  "snapshotType": "AutoGenerated",
  "timestamp": "2024-03-15T14:30:00Z"
}
```

---

### **POST /api/v1/content-items/{id}/rollback** - Rollback to Previous Version
**Description**: Rollback to previous snapshot version.

| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `targetVersion` | int | ✅ Yes | Target snapshot version to rollback to |

**Response** ✅ **WRAPPED** - Confirmation:
```json
{
  "success": true,
  "message": "Content item rolled back successfully",
  "data": {
    "id": "char-001",
    "title": "Geralt of Rivia",
    "version": 2,  // Rolled back to previous version
    "restoredFromVersion": 2,
    "timestamp": "2024-03-15T14:30:00Z"
  }
}
```

---

## **🔐 10. Task Management Endpoints** (Updated with ProjectTask Naming)

### **GET /api/v1/projects/{projectId}/tasks** - List All Tasks
**Description**: List all tasks for a project (filtered by difficulty, status).

| Path/Query Param | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `projectId` | Guid | ✅ Yes | Filter by project (required) |
| `status` | enum | ❌ No | Backlog, InProgress, Review, Done |
| `difficulty` | enum | ❌ No | Easy, Medium, Hard |
| `isQuickWin` | bool | ❌ No | ADHD-friendly filter |

**Response** ✅ **RAW** - Simple list retrieval:
```json
{
  "data": [
    {
      "id": "task-001",
      "title": "Write 3 opening dialogue lines",
      "projectId": "proj-001",
      "status": 1,  // InProgress
      "difficulty": 1,  // Easy
      "isQuickWin": true,
      "estimatedMinutes": 15,
      "assignedToUserId": null,
      "createdAt": "2024-03-15T10:00:00Z"
    }
  ],
  "pagination": {
    "currentPage": 1,
    "totalItems": 10,
    "totalPages": 1
  }
}
```

---

### **POST /api/v1/projects/{projectId}/tasks** - Create New Task
**Description**: Create new task for project.

| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `projectId` | Guid | Project ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `taskTitle` | string | ✅ Yes | Task title |
| `description` | string | ❌ No | Optional description |
| `status` | int | Default 0 | Backlog (0), InProgress (1), etc. |
| `difficulty` | int | Default 0 | Easy (0), Medium (1), Hard (2) |
| `isQuickWin` | bool | Default false | ADHD-friendly tag |

**Request Body** ✅ **RAW**:
```json
{
  "taskTitle": "Write 3 opening dialogue lines",
  "description": null,
  "status": 0,
  "difficulty": 0,
  "isQuickWin": true
}
```

**Response** ✅ **RAW** - Simple creation response:
```json
{
  "id": "task-001",
  "title": "Write 3 opening dialogue lines",
  "projectId": "proj-001",
  "status": 0,
  "difficulty": 0,
  "isQuickWin": true,
  "createdAt": "2024-03-15T10:00:00Z"
}
```

---

### **PUT /api/v1/projects/{projectId}/tasks/{taskId}** - Update Task
**Description**: Update task status, difficulty, etc.

| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `projectId` | Guid | Project ID |
| `taskId` | Guid | Task ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `status` | int | ❌ No | Update status (Backlog, InProgress, etc.) |
| `difficulty` | int | ❌ No | Update difficulty level |
| `isQuickWin` | bool | ❌ No | Toggle quick win badge |

**Request Body** ✅ **RAW**:
```json
{
  "status": 2,
  "difficulty": 1,
  "isQuickWin": true
}
```

**Response** ✅ **WRAPPED** - Confirmation:
```json
{
  "success": true,
  "message": "Task updated successfully",
  "data": {
    "id": "task-001",
    "title": "Write 3 opening dialogue lines",
    "status": 2,  // Updated to Review
    "difficulty": 1,  // Updated to Medium
    "isQuickWin": true,
    "lastModified": "2024-03-15T12:00:00Z"
  }
}
```

---

### **DELETE /api/v1/projects/{projectId}/tasks/{taskId}** - Delete Task (Admin Only)
**Description**: Delete task (admin only).

| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `projectId` | Guid | Project ID |
| `taskId` | Guid | Task ID |

**Response** ✅ **WRAPPED** - Confirmation:
```json
{
  "success": true,
  "message": "Task has been deleted successfully",
  "data": null
}
```

---

## **🔐 11. Story Outlining Endpoints** (Unchanged)

### **GET /api/v1/projects/{id}/sequences** - List Story Sequences (Chapters)
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |

**Response** ✅ **RAW**:
```json
{
  "data": [
    {
      "id": "seq-001",
      "sequenceName": "Chapter 1",
      "slug": "chapter-1",
      "description": "The protagonist discovers...",
      "published": true,
      "orderIndex": 1
    }
  ]
}
```

---

### **POST /api/v1/projects/{id}/sequences** - Create New Sequence (Chapter)
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `sequenceName` | string | ✅ Yes | e.g., "Chapter 1" |
| `slug` | string | ❌ No | Optional, auto-generated |

**Request Body** ✅ **RAW**:
```json
{
  "sequenceName": "Chapter 1",
  "slug": null
}
```

**Response** ✅ **WRAPPED** - Confirmation:
```json
{
  "success": true,
  "message": "Story sequence created successfully",
  "data": {
    "id": "seq-001",
    "sequenceName": "Chapter 1",
    "slug": "chapter-1",
    "orderIndex": 1
  }
}
```

---

## **🔐 12. External Reference Endpoints** (Unchanged)

### **GET /api/v1/content-items/{id}/references** - List External References
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

**Response** ✅ **RAW**:
```json
{
  "MetaInfoId": "char-001",
  "externalResources": [
    {
      "url": "https://notion.so/game-team/gdd",
      "title": "Official GDD Document",
      "type": "Document"
    },
    {
      "url": "https://pinterest.com/pin/xxx",
      "title": "Character Art Style Reference",
      "type": "Image"
    }
  ]
}
```

---

### **POST /api/v1/content-items/{id}/references** - Create External Reference
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `url` | string | ✅ Yes | External resource URL |
| `title` | string | ❌ No | Optional title |
| `type` | int | Default 0 | Document/Image/Video/Audio |

**Request Body** ✅ **RAW**:
```json
{
  "url": "https://notion.so/game-team/gdd",
  "title": "Official GDD Document",
  "type": 0
}
```

**Response** ✅ **WRAPPED** - Confirmation:
```json
{
  "success": true,
  "message": "External reference created successfully",
  "data": {
    "id": "ref-001",
    "url": "https://notion.so/game-team/gdd",
    "title": "Official GDD Document",
    "type": 0,  // Document
    "isActive": true,
    "createdAt": "2024-03-15T14:30:00Z"
  }
}
```

---

## **🔐 13. Tag Management Endpoints** (Unchanged)

### **GET /api/v1/content-items/{id}/tags** - List Tags for Content Item
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

**Response** ✅ **RAW**:
```json
{
  "MetaInfoId": "char-001",
  "tags": [
    {
      "tagId": "tag-001",
      "name": "MainCharacter",
      "slug": "main-character"
    },
    {
      "tagId": "tag-002",
      "name": "Human",
      "slug": "human"
    }
  ]
}
```

---

### **POST /api/v1/content-items/{id}/tags** - Add Tags to Content Item
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `tagIds` | array[Guid] | ✅ Yes | List of tag IDs to add |

**Request Body** ✅ **RAW**:
```json
{
  "tagIds": ["tag-001", "tag-002"]
}
```

**Response** ✅ **WRAPPED** - Confirmation:
```json
{
  "success": true,
  "message": "Tags added successfully",
  "data": {
    "MetaInfoId": "char-001",
    "addedTags": [
      {
        "tagId": "tag-001",
        "name": "MainCharacter"
      },
      {
        "tagId": "tag-002",
        "name": "Human"
      }
    ]
  }
}
```

---

## **🔐 14. Media Attachment Endpoints** (Unchanged)

### **POST /api/v1/content-items/{id}/media/upload** - Upload File to Media Area
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

| Form Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `file` | binary | ✅ Yes | Media file (max 100MB) |

**Response** ✅ **WRAPPED** - Confirmation:
```json
{
  "success": true,
  "message": "Media file uploaded successfully",
  "data": {
    "id": "att-001",
    "filename": "character-concept.png",
    "contentType": "image/png",
    "storagePath": "/uploads/2024/03/char-concept.png",
    "uploadedAt": "2024-03-15T14:30:00Z"
  }
}
```

---

### **GET /api/v1/content-items/{id}/media** - List Media Attachments
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

**Response** ✅ **RAW**:
```json
{
  "MetaInfoId": "char-001",
  "attachments": [
    {
      "id": "att-001",
      "filename": "character-concept.png",
      "contentType": "image/png",
      "storagePath": "/uploads/2024/03/char-concept.png",
      "uploadedAt": "2024-03-15T14:30:00Z"
    }
  ]
}
```

---

### **DELETE /api/v1/content-items/{id}/media/{attachmentId}** - Delete Media File
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |
| `attachmentId` | Guid | Media Attachment ID |

**Response** ✅ **WRAPPED** - Confirmation:
```json
{
  "success": true,
  "message": "Media file has been deleted successfully",
  "data": null
}
```

---

## **🔐 15. Export Endpoints** (Unchanged)

### **POST /api/v1/projects/{id}/export/json** - Export to JSON
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `sections` | array[string] | ❌ No | List of content types to export |
| `includeWatermark` | bool | ❌ No | Default: false |

**Request Body** ✅ **RAW**:
```json
{
  "sections": ["characters", "worlds", "mech"],
  "includeWatermark": false
}
```

**Response** ✅ **WRAPPED** - Confirmation:
```json
{
  "success": true,
  "message": "JSON export generated successfully",
  "data": {
    "project": {
      "id": "proj-001",
      "title": "The Dragon's Crown"
    },
    "MetaInfos": [
      {
        "id": "char-001",
        "title": "Geralt",
        "published": true,
        "attributes": [
          { "name": "Health", "value": 50 }
        ],
        "externalReferences": []
      }
    ],
    "version": "1.0"
  }
}
```

---

### **POST /api/v1/projects/{id}/export/csv** - Export to CSV (Unity Compatible)
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `contentType` | enum | ✅ Yes | Content type to export (e.g., Character) |
| `includeWatermark` | bool | ❌ No | Default: false |

**Request Body** ✅ **RAW**:
```json
{
  "contentType": "Character",
  "includeWatermark": false
}
```

**Response** ✅ **WRAPPED** - File download confirmation:
```json
{
  "success": true,
  "message": "CSV export generated successfully",
  "data": {
    "filename": "characters-2024-03-15.csv",
    "contentType": "text/csv",
    "downloadUrl": "/exports/characters-2024-03-15.csv"
  }
}
```

---

### **POST /api/v1/projects/{id}/export/xml-gdd** - Export to XML GDD (Unreal Compatible)
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `contentType` | enum | ✅ Yes | Content type to export (e.g., Character) |
| `includeWatermark` | bool | ❌ No | Default: false |

**Response** ✅ **WRAPPED** - File download confirmation:
```json
{
  "success": true,
  "message": "XML GDD export generated successfully",
  "data": {
    "filename": "characters-unreal-2024-03-15.xml",
    "contentType": "application/xml",
    "downloadUrl": "/exports/characters-unreal-2024-03-15.xml"
  }
}
```

---

### **POST /api/v1/projects/{id}/export/pdf** - Export to PDF (GDD Document)
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `sections` | array[string] | ❌ No | List of content types to export |
| `includeWatermark` | bool | ❌ No | Optional watermark for IP protection |
| `watermarkText` | string | ❌ No | Watermark text (e.g., "© YourCompany") |

**Request Body** ✅ **RAW**:
```json
{
  "sections": ["characters", "worlds"],
  "includeWatermark": true,
  "watermarkText": "© MyGameStudio"
}
```

**Response** ✅ **WRAPPED** - File download confirmation:
```json
{
  "success": true,
  "message": "PDF export generated successfully",
  "data": {
    "filename": "gdd-2024-03-15.pdf",
    "contentType": "application/pdf",
    "downloadUrl": "/exports/gdd-2024-03-15.pdf"
  }
}
```

---

## **🔐 16. Activity Feed Endpoint** (Unchanged)

### **GET /api/v1/projects/{id}/activity/feed** - Get Project Activity Timeline
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |

| Query Param | Type | Default | Notes |
| :--- | :--- | :--- | :--- |
| `days` | int | 7 | Number of days to retrieve |
| `eventType` | enum | All | Filter by event type (ContentCreated, TaskCompleted, etc.) |

**Response** ✅ **RAW**:
```json
{
  "data": [
    {
      "id": "act-001",
      "eventType": "ContentUpdated",
      "title": "Geralt Character Updated",
      "description": "User 'Jane Doe' updated the description...",
      "relatedEntityId": "char-001",
      "relatedEntityType": 1,  // MetaInfo
      "createdAt": "2024-03-15T14:30:00Z",
      "userId": "usr-001"
    }
  ]
}
```

---

## **🔐 17. Review Status Endpoints** (Unchanged)

### **GET /api/v1/content-items/{id}/review** - Get Review Status
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

**Response** ✅ **RAW**:
```json
{
  "MetaInfoId": "char-001",
  "reviewStatus": {
    "status": "Pending",
    "reviewedByUserId": null,
    "reviewComments": null,
    "reviewedAt": null
  }
}
```

---

### **PUT /api/v1/content-items/{id}/review** - Approve/Reject Content Item
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `status` | enum | ✅ Yes | Approved/Rejected/Pending |
| `reviewComments` | string | ❌ No | Review comments (required if rejecting) |

**Request Body** ✅ **RAW**:
```json
{
  "status": "Approved",
  "reviewComments": null
}
```

**Response** ✅ **WRAPPED** - Confirmation:
```json
{
  "success": true,
  "message": "Content item review updated successfully",
  "data": {
    "MetaInfoId": "char-001",
    "reviewStatus": {
      "status": "Approved",
      "reviewedByUserId": "usr-002",
      "reviewComments": "Great work on this character!",
      "reviewedAt": "2024-03-15T15:00:00Z"
    }
  }
}
```

---

## **🔐 18. Comment Endpoints** (Unchanged)

### **GET /api/v1/content-items/{id}/comments** - List Comments
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

| Query Param | Type | Default | Notes |
| :--- | :--- | :--- | :--- |
| `visibility` | enum | All | private, team-only, public |

**Response** ✅ **RAW**:
```json
{
  "data": [
    {
      "id": "comment-001",
      "MetaInfoId": "char-001",
      "commentedByUserId": "usr-002",
      "commentText": "This character needs better backstory.",
      "visibility": "private",
      "createdAt": "2024-03-15T14:30:00Z"
    }
  ]
}
```

---

### **POST /api/v1/content-items/{id}/comments** - Create Comment
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `commentText` | string | ✅ Yes | Comment text (Markdown/HTML) |
| `visibility` | enum | Default "private" | private, team-only, public |

**Request Body** ✅ **RAW**:
```json
{
  "commentText": "This character needs better backstory.",
  "visibility": "team-only"
}
```

**Response** ✅ **WRAPPED** - Confirmation:
```json
{
  "success": true,
  "message": "Comment created successfully",
  "data": {
    "id": "comment-001",
    "MetaInfoId": "char-001",
    "commentedByUserId": "usr-002",
    "commentText": "This character needs better backstory.",
    "visibility": "team-only",
    "createdAt": "2024-03-15T14:30:00Z"
  }
}
```

---

## **🔐 19. Search Endpoints** (Unchanged)

### **POST /api/v1/search/content-items** - Search Content Items
| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `query` | string | ✅ Yes | Search query (title, description, slug) |
| `contentType` | enum | ❌ No | Filter by content type |
| `publishedOnly` | bool | Default false | Only published items |

**Request Body** ✅ **RAW**:
```json
{
  "query": "dragon",
  "contentType": "Character",
  "publishedOnly": true
}
```

**Response** ✅ **RAW**:
```json
{
  "data": [
    {
      "id": "char-001",
      "title": "Geralt of Rivia",
      "slug": "geralt-of-rivia",
      "contentType": 1,
      "description": "...",
      "published": true
    }
  ],
  "pagination": {
    "currentPage": 1,
    "totalItems": 5,
    "totalPages": 1
  }
}
```

---

## **🔐 20. API Rate Limiting** (Unchanged)

### **Rate Limit Headers (in all responses)**
```http
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1705324800
```

### **Rate Limit Policy**
- Default limit: 100 requests per minute per IP
- Export endpoints: 5 requests per minute per IP
- Authentication endpoints: 1000 requests per hour per IP (for OAuth)

---

## **🔐 21. API Versioning** (Unchanged)

### **Current Version**: v1
All endpoints follow `/api/v1/{resource}` convention. To upgrade to v2:
- New major version: `/api/v2/{resource}`
- Deprecation headers for v1 endpoints

---

## **🔐 22. Response Pattern Rules** (NEW Section)

### **Hybrid Response Pattern Summary:**

| Endpoint Type | Response Pattern | Example |
| :--- | :--- | :--- |
| **List Operations** (GET /items) | ✅ **RAW** - Simple data retrieval | `{ "data": [...] }` |
| **Create Operations** (POST /items) | ✅ **RAW** - Simple creation response | `{ "id": "...", "title": "..." }` |
| **Update Operations** (PUT /items/{id}) | ❓ **Context-dependent**: <br>✅ **WRAPPED** if confirmation needed<br>✅ **RAW** if data returned | See examples above |
| **Delete Operations** (DELETE /items/{id}) | ✅ **WRAPPED** - Confirmation only | `{ "success": true, "message": "..." }` |
| **Upload Operations** (POST /upload) | ✅ **WRAPPED** - File upload confirmation | `{ "success": true, "data": { "filename": "..." } }` |
| **Export Operations** (POST /export/*) | ✅ **WRAPPED** - File download confirmation | `{ "success": true, "message": "...", "data": { "downloadUrl": "..." } }` |

### **When to Use Wrapped Response:**
✅ User-triggered confirmations (e.g., "File uploaded successfully")  
✅ Side effects (e.g., email sent, file generated)  
✅ Batch operations with multiple results  

### **When to Use Raw Response:**
✅ Simple data retrieval (list, get single item)  
✅ Simple creation/update responses without confirmation needs  
✅ Data that doesn't require success/message metadata  

---

## **🔐 23. API Response Examples** (Unchanged)

### **Success Response (200/201)**
```json
// RAW - Simple data retrieval
{
  "id": "char-001",
  "title": "Geralt of Rivia"
}

// WRAPPED - User-triggered confirmation
{
  "success": true,
  "message": "Content updated successfully",
  "data": {
    "id": "char-001",
    "title": "Geralt of Rivia"
  }
}
```

### **Error Response (4xx)**
```json
{
  "success": false,
  "errors": ["Description cannot be empty"],
  "message": "Validation failed"
}
```

---

## **🔐 24. Summary Table: Complete API Endpoints** (Updated with Domain Clustering)

| Category | Endpoint Count | Key Features | Notes |
| :--- | :--- | :--- | :--- |
| **Authentication** | 4 | OAuth, Password, 2FA, Recovery codes | Wrapped responses for user-triggered actions |
| **Projects** | 5 | CRUD with template support | Project model added ✅ |
| **Content Items** | 10 | Full lifecycle with version control | Hybrid response pattern applied ✅ |
| **Story Outlining** | 2 | Chapter/sequence management | Wrapped for creation confirmation |
| **Dialogue Trees** | 2 | Branch/nodes creation | Wrapped for creation confirmation |
| **External References** | 2 | Structured external resource links | Raw for list, wrapped for creation |
| **Tag Management** | 3 | Tag CRUD and associations | Raw for list, wrapped for creation |
| **Media Upload** | 3 | File upload, listing, deletion | Wrapped for all operations |
| **Task Management** | 4 | Flat structure with ADHD-friendly features | **Renamed to ProjectTask** ✅ |
| **Export** | 4 | JSON/CSV/XML/PDF formats | Wrapped for all export operations |
| **Activity Feed** | 1 | Project activity timeline | Raw response pattern |
| **Review Status** | 2 | Approval workflow | Wrapped for approval actions |
| **Comment Endpoints** | 2 | Content comments with visibility control | Raw for list, wrapped for creation |
| **Search** | 1 | Full-text search with filters | Raw response pattern |

---

## **🔐 25. Updated DTO Design Patterns** (Domain Clustering)

### **Projects Domain DTOs:**
```csharp
// CreateProjectDto.cs (in src/Gadema.Core/Dtos/Projects/)
public class CreateProjectDto
{
    [Required]
    [Display(Name = "Project Title")]
    public string Title { get; set; } = "";

    [MaxLength(128)]
    [Display(Name = "URL Slug")]
    public string? Slug { get; set; } = null!;

    [EnumDataType(typeof(ProjectTemplateTypeEnum))]
    [Display(Name = "Template Type")]
    public int TemplateTypeId { get; set; }
}
```

### **Tasks Domain DTOs (Renamed):**
```csharp
// ProjectTaskCreateDto.cs (in src/Gadema.Core/Dtos/Tasks/)
public class ProjectTaskCreateDto
{
    [MaxLength(256)]
    [Display(Name = "Task Title")]
    public string TaskTitle { get; set; } = "";

    [MaxLength(4096)]
    [Display(Name = "Description")]
    public string? Description { get; set; } = null!;

    public int Status { get; set; }  // Backlog(0), InProgress(1), etc.

    public int Difficulty { get; set; }  // Easy(0), Medium(1), Hard(2)

    [Display(Name = "Is Quick Win?")]
    public bool IsQuickWin { get; set; } = false;
}

// UpdateProjectTaskDto.cs (in src/Gadema.Core/Dtos/Tasks/)
public class UpdateProjectTaskDto
{
    public int Status { get; set; }
    
    public int Difficulty { get; set; }
    
    public bool IsQuickWin { get; set; }
}

// ProjectTaskResponseDto.cs (in src/Gadema.Core/Dtos/Tasks/)
public class ProjectTaskResponseDto
{
    [Display(Name = "ID")]
    public Guid Id { get; set; }

    [Display(Name = "Task Title")]
    public string TaskTitle { get; set; } = "";

    public int Status { get; set; }

    public int Difficulty { get; set; }

    [Display(Name = "Is Quick Win?")]
    public bool IsQuickWin { get; set; }

    [Display(Name = "Created At")]
    public DateTime CreatedAt { get; set; }
}
```

---

## **🐱 Summary**

**This updated API-CONTRACTS.md documentation for GaDeMa v0.1 Pre-Release MVP now includes:**

✅ Complete ~35 REST endpoints with request/response formats  
✅ Authentication requirements and authorization logic  
✅ Error handling patterns (all 4xx responses wrapped)  
✅ Hybrid response pattern (raw vs. wrapped based on endpoint type)  
✅ DTO design patterns with domain clustering (Projects/, Tasks/, Content/)  
✅ Model renaming (Task → ProjectTask to avoid System.Threading.Task ambiguity) ✅  
✅ View mode separation logic in examples  
✅ API rate limiting headers and policies  
✅ Domain-aware folder structure reference  

**Total Implementation Effort**: ~35 endpoints with hybrid response patterns  
**Version**: v0.1 (Pre-Release MVP)  
**Status**: Production-Ready Architecture ✅

---
