# 📄 **API-CONTRACTS.md** - Complete REST Endpoint Documentation  
**GaDeMa – Game Development Management Application (v0.1 Pre-Release MVP)**  
**Status**: Production-Ready Architecture with Full Endpoint Specifications  

---

## **📋 Overview**

This document contains the complete API contract definition for GaDeMa v0.1, including:
- ✅ All REST endpoint specifications (Authentication, Projects, Content, ProjectTasks, Export)
- ✅ Request/response formats with examples
- ✅ Authentication and authorization requirements
- ✅ Error handling patterns
- ✅ DTO design patterns and validation rules
- ✅ Pagination conventions
- ✅ View mode separation logic

**Total Endpoints**: ~30+ | **Framework**: ASP.NET Core Web API | **Base URL**: `/api/v1/`

---

## **📁 File Structure Reference**

```bash
src/
├── Gadema.Api/Controllers/    # Endpoint implementations
├── Gadema.Core/Dtos/          # Request/response DTOs
└── docs/API-CONTRACTS.md        # This documentation file
```

---

## **🔐 1. Base URL Structure**

```
Production: https://gaema-api.com/api/v1/
Staging:    https://staging.gaema-api.com/api/v1/
Local Dev:  http://localhost:5000/api/v1/
```

**All endpoints follow the v1 versioning convention**: `/api/v1/{resource}`

---

## **🔐 2. Authentication & Authorization**

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

## **🔐 3. Error Response Format**

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

## **🔐 4. Pagination Pattern**

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

## **🔐 5. View Mode Separation**

API endpoints support both `PrivateWriting` and `Presentation` view modes for MetaInfo responses:

### **View Mode Query Parameter**
| Parameter | Type | Default | Notes |
| :--- | :--- | :--- | :--- |
| `viewMode` | enum | PrivateWriting | PrivateWriting or Presentation |

### **Response Differences**

**PrivateWriting Mode**: Full admin interface (includes all fields + version history)

**Presentation Mode**: Clean public view (only published fields, no admin details)

```csharp
// Controller action parameter:
[FromQuery] public ViewModeEnum ViewMode { get; set; } = ViewModeEnum.PrivateWriting;
```

---

## **🔐 6. DTO Design Patterns**

### **Naming Convention Rules**
- ✅ `CreateXDto` for creation operations (e.g., `CreateProjectDto`)
- ✅ `UpdateXDto` for partial updates (e.g., `UpdateMetaInfoDto`)
- ✅ `ResponseXDto` for API responses (e.g., `MetaInfoResponseDto`)
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

## **🔐 7. Authentication Endpoints**

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

**Response**:
```json
{
  "tokenType": "Bearer",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresInSeconds": 3600,
  "refreshToken": "dGVzdC1yZWZyZXNoLXRva2Vu",
  "user": {
    "id": "usr-001",
    "name": "Jane Doe",
    "email": "jane@example.com",
    "googleSubjectId": null
  }
}
```

---

### **POST /api/v1/auth/callback/google** - Google OAuth Callback
**Description**: Complete Google OAuth authentication flow.

| Query Param | Type | Notes |
| :--- | :--- | :--- |
| `code` | string | OAuth authorization code from Google |

**Response**: Same as Sign-In endpoint above.

---

### **POST /api/v1/auth/signin-with-2fa** - Two-Factor Authentication
**Description**: Complete authentication with 2FA token.

| Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `email` | string | ✅ Yes | User email |
| `password` | string | ❌ No | For traditional login |
| `twoFactorToken` | string | ✅ Yes | TOTP token (6-digit code) |

**Response**: Same as Sign-In endpoint, but requires valid 2FA token.

---

### **GET /api/auth/recovery-codes** - View Recovery Codes
**Description**: Retrieve recovery codes for current user account.

| Query Param | Type | Notes |
| :--- | :--- | :--- |
| `format` | string | "csv" (default) or "text" |

**Response**:
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

### **DELETE /api/v1/auth/2fa/disable** - Disable 2FA
**Description**: Disable two-factor authentication for current user account.

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `twoFactorToken` | string | ✅ Yes | Current TOTP token for verification |

**Response**:
```json
{
  "success": true,
  "message": "2FA has been disabled successfully"
}
```

---

## **🔐 8. Projects Endpoints**

### **GET /api/v1/projects** - List Projects
**Description**: Get a list of all projects (filtered by user/team ownership).

| Query Param | Type | Default | Notes |
| :--- | :--- | :--- | :--- |
| `page` | int | 1 | Page number |
| `pageSize` | int | 20 | Items per page |
| `status` | enum | All | Draft, InProgress, Published, Archived |
| `visibility` | enum | All | Public/Private |
| `search` | string | - | Search by title/slug |

**Response**:
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

**Request Body**:
```json
{
  "title": "The Dragon's Crown",
  "slug": null,
  "templateId": null,
  "visibility": 2
}
```

**Response**:
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

### **PUT/PATCH /api/v1/projects/{id}** - Update Project
**Description**: Update project ownership, visibility, publish, transfer ownership.

| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `visibility` | int | ❌ No | 1 (Private) or 2 (Public) |
| `enableUserRegistration` | bool | ❌ No | Enable self-registration |
| `allowManualInvites` | bool | ❌ No | Allow team invitations |

**Request Body**:
```json
{
  "visibility": 2,
  "enableUserRegistration": true,
  "allowManualInvites": false
}
```

**Response**:
```json
{
  "id": "proj-001",
  "title": "The Dragon's Crown",
  "visibility": 2,  // Updated to Public
  "enableUserRegistration": true,  // Now enabled
  "lastModified": "2024-03-15T12:00:00Z"
}
```

---

### **DELETE /api/v1/projects/{id}** - Transfer/Delete Project
**Description**: Transfer or delete project.

| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |

**Response**:
```json
{
  "success": true,
  "message": "Project has been transferred to: [username]",
  "transferredAt": "2024-03-15T12:00:00Z"
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

**Request Body**:
```json
{
  "tokenName": "CI/CD Pipeline",
  "permissions": ["Export", "Read"],
  "expiresAt": null
}
```

**Response**:
```json
{
  "id": "tok-001",
  "name": "CI/CD Pipeline",
  "hash": "dG9rZW4tY2hhcy0xMjM=",  // Encoded hash (not plain text)
  "isActive": true,
  "expiresAt": null,
  "permissions": ["Export", "Read"],
  "createdAt": "2024-03-15T14:30:00Z"
}
```

---

### **GET /api/v1/projects/{id}/tokens** - List Project Tokens
**Description**: List all API tokens for a project.

| Query Param | Type | Default | Notes |
| :--- | :--- | :--- | :--- |
| `includeUsage` | bool | false | Include usage logs |

**Response**:
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

**Response**:
```json
{
  "success": true,
  "message": "Token has been revoked successfully",
  "revokedAt": "2024-03-15T14:30:00Z"
}
```

---

## **🔐 9. Content Items Endpoints**

### **GET /api/v1/content/items** - List Content Items
**Description**: List content for project (filtered by published, status, tags).

| Path/Query Param | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `projectId` | Guid | ❌ No | Filter by project (required in practice) |
| `contentType` | enum | ❌ No | Filter by content type |
| `status` | enum | ❌ No | Filter by status |
| `published` | bool | ❌ No | Default: true (for public view) |
| `viewMode` | enum | ❌ No | PrivateWriting or Presentation |

**Response**:
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

**Response**:
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

**Request Body**:
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

**Response**:
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

**Request Body**:
```json
{
  "description": "...updated...",
  "published": true,
  "viewMode": "Presentation"
}
```

**Response**:
```json
{
  "id": "char-001",
  "title": "Geralt of Rivia",
  "description": "...updated...",
  "published": true,
  "viewMode": "Presentation",
  "version": 2,
  "status": "Published"
}
```

---

### **DELETE /api/v1/content/items/{id}** - Delete Content Item (Admin Only)
**Description**: Delete content item (admin only).

| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

**Response**:
```json
{
  "success": true,
  "message": "Content item has been deleted successfully"
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

**Response**:
```json
{
  "id": "att-001",
  "filename": "character-concept.png",
  "contentType": "image/png",
  "storagePath": "/uploads/2024/03/char-concept.png",
  "uploadedAt": "2024-03-15T14:30:00Z"
}
```

---

### **POST /api/v1/content-items/{id}/autosave** - Auto-Save Snapshot
**Description**: Auto-save snapshot for versioning.

| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

**Response**:
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

**Response**:
```json
{
  "id": "char-001",
  "title": "Geralt of Rivia",
  "version": 2,  // Rolled back to previous version
  "restoredFromVersion": 2,
  "timestamp": "2024-03-15T14:30:00Z"
}
```

---

## **🔐 10. Story Outlining Endpoints**

### **GET /api/v1/projects/{id}/sequences** - List Story Sequences (Chapters)
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |

**Response**:
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

**Request Body**:
```json
{
  "sequenceName": "Chapter 1",
  "slug": null
}
```

**Response**:
```json
{
  "id": "seq-001",
  "sequenceName": "Chapter 1",
  "slug": "chapter-1",
  "orderIndex": 1
}
```

---

## **🔐 11. Dialogue Trees Endpoints**

### **GET /api/v1/projects/{id}/dialogue/branches** - List Dialogue Branches (Tree Structure)
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |

**Response**:
```json
{
  "data": [
    {
      "id": "branch-001",
      "title": "Main Storyline",
      "slug": "main-storyline",
      "visualNodeImageUri": "/uploads/character-concept.png",
      "characterIconUri": null,
      "isRoot": true,
      "orderIndex": 0
    }
  ]
}
```

---

### **POST /api/v1/projects/{id}/dialogue/branches** - Create New Dialogue Branch
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `title` | string | ✅ Yes | Branch title |
| `slug` | string | ❌ No | Optional, auto-generated |
| `visualNodeImageUri` | string | ❌ No | Image URI for visual node |
| `characterIconUri` | string | ❌ No | Character icon URI |

**Request Body**:
```json
{
  "title": "Main Storyline",
  "slug": null,
  "visualNodeImageUri": "/uploads/character-concept.png",
  "characterIconUri": null
}
```

---

## **🔐 12. External Reference Endpoints**

### **GET /api/v1/content-items/{id}/references** - List External References
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

**Response**:
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

**Request Body**:
```json
{
  "url": "https://notion.so/game-team/gdd",
  "title": "Official GDD Document",
  "type": 0
}
```

**Response**:
```json
{
  "id": "ref-001",
  "url": "https://notion.so/game-team/gdd",
  "title": "Official GDD Document",
  "type": 0,  // Document
  "isActive": true,
  "createdAt": "2024-03-15T14:30:00Z"
}
```

---

## **🔐 13. Task Management Endpoints** (continued)

### **GET /api/v1/projecttasks** - List All Tasks
| Query Param | Type | Default | Notes |
| :--- | :--- | :--- | :--- |
| `projectId` | Guid | ❌ No | Filter by project |
| `status` | enum | All | Backlog, InProgress, Review, Done |
| `difficulty` | enum | All | Easy, Medium, Hard |
| `isQuickWin` | bool | false | ADHD-friendly filter |

**Response**:
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

### **POST /api/v1/projecttasks** - Create New Task
| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `projectId` | Guid | ✅ Yes | Project ID |
| `projecttaskTitle` | string | ✅ Yes | ProjectTask title |
| `description` | string | ❌ No | Optional description |
| `status` | int | Default 0 | Backlog (0), InProgress (1), etc. |
| `difficulty` | int | Default 0 | Easy (0), Medium (1), Hard (2) |
| `isQuickWin` | bool | Default false | ADHD-friendly tag |

**Request Body**:
```json
{
  "projectId": "proj-001",
  "projecttaskTitle": "Write 3 opening dialogue lines",
  "description": null,
  "status": 0,
  "difficulty": 0,
  "isQuickWin": true
}
```

**Response**:
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

### **PUT /api/v1/projecttasks/{id}** - Update Task
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Task ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `status` | int | ❌ No | Update status (Backlog, InProgress, etc.) |
| `difficulty` | int | ❌ No | Update difficulty level |
| `isQuickWin` | bool | ❌ No | Toggle quick win badge |

**Request Body**:
```json
{
  "status": 2,
  "difficulty": 1,
  "isQuickWin": true
}
```

**Response**:
```json
{
  "id": "task-001",
  "title": "Write 3 opening dialogue lines",
  "status": 2,  // Updated to Review
  "difficulty": 1,  // Updated to Medium
  "isQuickWin": true,
  "lastModified": "2024-03-15T12:00:00Z"
}
```

---

### **DELETE /api/v1/projecttasks/{id}** - Delete Task (Admin Only)
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Task ID |

**Response**:
```json
{
  "success": true,
  "message": "ProjectTask has been deleted successfully"
}
```

---


## **🔐 14. Export Endpoints** (continued)

### **POST /api/v1/projects/{id}/export/json** - Export to JSON
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `sections` | array[string] | ❌ No | List of content types to export |
| `includeWatermark` | bool | ❌ No | Default: false |

**Request Body**:
```json
{
  "sections": ["characters", "worlds", "mech"],
  "includeWatermark": false
}
```

**Response**:
```json
{
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
        { "name": "Health", "value": 50 },
        { "name": "AttackPower", "value": 10 }
      ],
      "externalReferences": [
        {
          "url": "https://notion.so/game-team/gdd",
          "title": "Official GDD Document"
        }
      ]
    }
  ],
  "version": "1.0"
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

**Request Body**:
```json
{
  "contentType": "Character",
  "includeWatermark": false
}
```

**Response**: CSV file with Unity-compatible character sheet data.

---

### **POST /api/v1/projects/{id}/export/xml-gdd** - Export to XML GDD (Unreal Compatible)
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |

| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `contentType` | enum | ✅ Yes | Content type to export (e.g., Character) |
| `includeWatermark` | bool | ❌ No | Default: false |

**Response**: XML file for Unreal Engine GDD import.

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

**Request Body**:
```json
{
  "sections": ["characters", "worlds"],
  "includeWatermark": true,
  "watermarkText": "© MyGameStudio"
}
```

**Response**: PDF file with publication-ready GDD document.

---

## **🔐 15. Activity Feed Endpoint**

### **GET /api/v1/projects/{id}/activity/feed** - Get Project Activity Timeline
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Project ID |

| Query Param | Type | Default | Notes |
| :--- | :--- | :--- | :--- |
| `days` | int | 7 | Number of days to retrieve |
| `eventType` | enum | All | Filter by event type (ContentCreated, TaskCompleted, etc.) |

**Response**:
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

## **🔐 16. Review Status Endpoints**

### **GET /api/v1/content-items/{id}/review** - Get Review Status
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

**Response**:
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

**Request Body**:
```json
{
  "status": "Approved",
  "reviewComments": null
}
```

**Response**:
```json
{
  "MetaInfoId": "char-001",
  "reviewStatus": {
    "status": "Approved",
    "reviewedByUserId": "usr-002",
    "reviewComments": "Great work on this character!",
    "reviewedAt": "2024-03-15T15:00:00Z"
  }
}
```

---

## **🔐 17. Comment Endpoints**

### **GET /api/v1/content-items/{id}/comments** - List Comments
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

| Query Param | Type | Default | Notes |
| :--- | :--- | :--- | :--- |
| `visibility` | enum | All | private, team-only, public |

**Response**:
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

**Request Body**:
```json
{
  "commentText": "This character needs better backstory.",
  "visibility": "team-only"
}
```

**Response**:
```json
{
  "id": "comment-001",
  "MetaInfoId": "char-001",
  "commentedByUserId": "usr-002",
  "commentText": "This character needs better backstory.",
  "visibility": "team-only",
  "createdAt": "2024-03-15T14:30:00Z"
}
```

---

## **🔐 18. Tag Endpoints**

### **GET /api/v1/content-items/{id}/tags** - List Tags for Content Item
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

**Response**:
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

**Request Body**:
```json
{
  "tagIds": ["tag-001", "tag-002"]
}
```

**Response**:
```json
{
  "success": true,
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
```

---

## **🔐 19. Media Attachment Endpoints**

### **POST /api/v1/content-items/{id}/media/upload** - Upload File to Media Area
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

| Form Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `file` | binary | ✅ Yes | Media file (max 100MB) |

**Response**:
```json
{
  "id": "att-001",
  "filename": "character-concept.png",
  "contentType": "image/png",
  "storagePath": "/uploads/2024/03/char-concept.png",
  "uploadedAt": "2024-03-15T14:30:00Z"
}
```

---

### **GET /api/v1/content-items/{id}/media** - List Media Attachments
| Path Param | Type | Notes |
| :--- | :--- | :--- |
| `id` | Guid | Content Item ID |

**Response**:
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

**Response**:
```json
{
  "success": true,
  "message": "Media file has been deleted successfully"
}
```

---

## **🔐 20. Search Endpoints**

### **POST /api/v1/search/content-items** - Search Content Items
| Body Field | Type | Required | Notes |
| :--- | :--- | :--- | :--- |
| `query` | string | ✅ Yes | Search query (title, description, slug) |
| `contentType` | enum | ❌ No | Filter by content type |
| `publishedOnly` | bool | Default false | Only published items |

**Request Body**:
```json
{
  "query": "dragon",
  "contentType": "Character",
  "publishedOnly": true
}
```

**Response**:
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

## **🔐 21. API Rate Limiting**

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

## **🔐 22. View Mode Separation in API**

### **PrivateWriting Mode Response**:
```json
{
  "id": "char-001",
  "title": "Geralt of Rivia",
  "description": "...",
  "published": false,
  "viewMode": "PrivateWriting",
  "version": 3,
  "status": "Draft"
}
```

### **Presentation Mode Response**:
```json
{
  "id": "char-001",
  "title": "Geralt of Rivia",
  "published": true,
  "viewMode": "Presentation",
  "description": "...",
  "version": 3,
  "slug": "geralt-of-rivia"
}
```

---

## **🔐 23. API Versioning**

### **Current Version**: v1
All endpoints follow `/api/v1/{resource}` convention. To upgrade to v2:
- New major version: `/api/v2/{resource}`
- Deprecation headers for v1 endpoints

---

## **🔐 24. Summary Table: Complete API Endpoints**

| Category | Endpoint Count | Key Features |
| :--- | :--- | :--- |
| **Authentication** | 4 | OAuth, Password, 2FA, Recovery codes |
| **Projects** | 5 | CRUD with template support |
| **Content Items** | 10 | Full lifecycle with version control |
| **Story Outlining** | 2 | Chapter/sequence management |
| **Dialogue Trees** | 2 | Branch/nodes creation |
| **External References** | 2 | Structured external resource links |
| **Task Management** | 4 | Flat structure with ADHD-friendly features |
| **Export** | 4 | JSON, CSV, XML, PDF formats |
| **Activity Feed** | 1 | Project activity timeline |
| **Review Status** | 2 | Approval workflow |
| **Comment Endpoints** | 2 | Content comments with visibility control |
| **Tag Management** | 3 | Tag CRUD and associations |
| **Media Upload** | 3 | File upload, listing, deletion |
| **Search** | 1 | Full-text search with filters |

---

## **🔐 25. API Response Headers (Standard)**

```http
Content-Type: application/json
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1705324800
Cache-Control: no-cache, no-store, must-revalidate
Pragma: no-cache
Expires: 0
```

---

## **🔐 26. API Response Examples**

### **Success Response (200 OK)**
```json
{
  "success": true,
  "data": [...]
}
```

### **Created Response (201 Created)**
```json
{
  "id": "char-001",
  "title": "Geralt of Rivia"
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
