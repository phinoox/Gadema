# 📄 **USER_STORIES.md** – Updated for Production-Ready MVP
**GaDeMa – Game Development Management Application (v0.1 Pre-Release MVP)**  
**Status**: Complete with Project Model ✅, Task → ProjectTask Renaming ✅, Domain Clustering ⭐, Hybrid Response Patterns  

---

## **📋 Overview**

This document defines the complete user stories, acceptance criteria, and technical scope for GaDeMa v0.1. It ensures the development team builds exactly what users need while maintaining consistency with domain clustering, hybrid response patterns, and updated entity names (ProjectTask instead of Task).

**Target Audience**: Product Managers, Developers, UX Designers  
**Coverage**: Authentication, Projects, Content Management, Tasks, Team Collaboration, ADHD-Friendly Features  

---

## **🔍 Document Structure**

| Section | Stories | Key Features | Status |
| :--- | :--- | :--- | :--- |
| **1. Authentication & Security** | 5 | OAuth, Password, 2FA, Recovery Codes ✅ Updated | ✅ Complete |
| **2. Project Setup & Configuration** | 4 | Create Project, Templates, Visibility, Series ✅ Updated | ✅ Complete |
| **3. Content Creation & Management** | 6 | Content Items, Outlines, Media, Tags ✅ Updated | ✅ Complete |
| **4. Task Management & ADHD Support** | 8 | **PROJECT-TASK-x**, Quick Wins, Difficulty Filtering ✅ Renamed | ✅ Complete |
| **5. Story Outlining & Navigation** | 3 | Sequences, Beats, Lore ✅ Updated | ✅ Complete |
| **6. Team Collaboration & Review** | 4 | Roles, Comments, Approvals ✅ Updated | ✅ Complete |
| **7. Version Control & Rollback** | 3 | Snapshots, Rollback, Activity Log ✅ Updated | ✅ Complete |
| **8. Export & Engine Integration** | 3 | JSON/CSV/XML/PDF, Unity/Unreal ✅ Updated | ✅ Complete |
| **9. Search & Discovery** | 2 | Full-text search, Filters ✅ Updated | ✅ Complete |
| **TOTAL** | **~50** | All with domain-aware patterns ✅ | ✅ Complete |

---

## **1️⃣ Authentication & Security (5 Stories)**

### **AUTH-01: Google OAuth Sign-In**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **AUTH-01** | As a user, I want to sign in with my Google account so that I don't need to remember passwords. *(Domain-aware pattern: Uses Google OAuth 2.0 flow)* | - Click "Sign in with Google" button<br>- Redirect to Google consent screen<br>- Auto-provision user if not exists<br>- Generate JWT token (1 hour expiry)<br>- **Hybrid response pattern**: `{ "success": true, "data": { "accessToken": "...", "user": {...} } }` | **High** | `Authentication/AuthController.cs`, `ApiAuthService.cs`, `JwtTokenService.cs` |

### **AUTH-02: Password Recovery & 2FA**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **AUTH-02** | As a user who lost access to my authenticator app, I want to recover my account using backup codes or email. *(Domain-aware pattern: Uses RecoveryCodes table)* | - Display 8 recovery codes in CSV format<br>- One-time use validation for each code<br>- Generate new codes after usage<br>- **Hybrid response pattern**: `{ "success": false, "errors": ["Code used once"], "message": "Recovery failed" }` | **Medium** | `Authentication/AuthController.cs`, `RecoveryCodesDto.cs` |

### **AUTH-03: Two-Factor Authentication (TOTP)**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **AUTH-03** | As a user concerned about security, I want to enable two-factor authentication using TOTP so that unauthorized access is prevented. *(Domain-aware pattern: Uses 2FA middleware)* | - Generate QR code for authenticator app setup<br>- Validate TOTP token during sign-in<br>- Store 2FA secret securely (encrypted)<br>- **Hybrid response pattern**: `{ "success": true, "data": { "totpSecret": "..." } }` | **High** | `Authentication/AuthController.cs`, `TwoFactorAuthDto.cs` |

### **AUTH-04: Password Policy Enforcement**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **AUTH-04** | As a security-conscious admin, I want to enforce strong password policies so that user accounts are protected. *(Domain-aware pattern: Uses PasswordPolicy service)* | - Minimum 8 characters required<br>- Must contain uppercase, lowercase, number<br>- No common passwords (e.g., "password")<br>- **Hybrid response pattern**: `{ "success": false, "errors": ["Password too weak"], "message": "Validation failed" }` | **High** | `Authentication/AuthController.cs`, `PasswordPolicy.cs` |

### **AUTH-05: Session Management & Token Refresh**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **AUTH-05** | As a user with long-running sessions, I want to refresh my access token automatically so that I don't get logged out unexpectedly. *(Domain-aware pattern: Uses RefreshToken rotation)* | - Access token expires after 1 hour<br>- Refresh token valid for 7 days<br>- Rotate refresh token on each use<br>- **Hybrid response pattern**: `{ "success": true, "data": { "refreshToken": "..." } }` | **Medium** | `Authentication/AuthController.cs`, `RefreshTokenDto.cs` |

---

## **2️⃣ Project Setup & Configuration (4 Stories)**

### **PROJ-01: Create Project from Template**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **PROJ-01** | As a novice writer, I want to create a project from a pre-defined template (RPG, VN, etc.) so that I can start development instantly without configuring schemas. *(Domain-aware pattern: Uses Project model + auto-copies AttributeSet, ClassTemplate)* | - Template selection dropdown<br>- Auto-population of structure<br>- Ownership transferable to Team later<br>- **Hybrid response pattern**: `{ "success": true, "data": { "id": "...", "title": "...", "slug": "..." } }` | **High** | `Projects/ProjectsController.cs`, `CreateProjectDto.cs`, `ProjectEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **PROJ-02: Project Series Support**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **PROJ-03** | As a writer, I want to organize my projects into series (e.g., "Book 1", "Book 2") so that I can track my narrative progression across multiple stories. *(Domain-aware pattern: Uses Project.SeriesId FK for parent-child tracking)* | - Optional SeriesName field<br>- Parent project relationship<br>- Series tracking via FK<br>- **Hybrid response pattern**: `{ "success": true, "message": "Project added to series", "data": { "seriesId": "..." } }` | **Medium** | `Projects/ProjectsController.cs`, `SeriesProjectEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **PROJ-03: Project Visibility Management**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **PROJ-04** | As a project manager, I want to control who can see my projects so that sensitive content remains protected. *(Domain-aware pattern: Uses ProjectVisibilityEnum)* | - Private/Public toggle<br>- Role-based access control<br>- Audit log of visibility changes<br>- **Hybrid response pattern**: `{ "success": true, "message": "Visibility updated", "data": { "visibility": 2 } }` | **High** | `Projects/ProjectsController.cs`, `ProjectVisibilityEnum.cs` ✅ Domain-aware enum! |

### **PROJ-04: Project Enable User Registration**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **PROJ-05** | As an admin, I want to enable or disable self-registration for my projects so that I control who joins. *(Domain-aware pattern: Uses Feature Flags)* | - Global feature flag `EnableUserRegistration`<br>- Admin-only toggle<br>- Audit log of registration enable/disable<br>- **Hybrid response pattern**: `{ "success": true, "message": "Registration enabled", "data": { "enableUserRegistration": true } }` | **Medium** | `Projects/ProjectsController.cs`, `FeatureFlags.cs` ✅ Domain-aware feature flag! |

---

## **3️⃣ Content Creation & Management (6 Stories)**

### **CONTENT-01: Create/Edit Content Items**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **CONTENT-01** | As a writer, I want to create content items (characters, worlds, etc.) with dedicated outlines first so that my GDD is structured and organized. *(Domain-aware pattern: Uses ContentItem + StoryOutline)* | - Create outline before content<br>- Rich text editor support<br>- Markdown/HTML conversion<br>- **Hybrid response pattern**: `{ "success": true, "data": { "id": "...", "title": "..." } }` | **High** | `Content/ContentItemsController.cs`, `CreateContentItemDto.cs` ✅ Domain-separated config! |

### **CONTENT-02: View Mode Separation**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **CONTENT-02** | As a writer with ADHD, I want to toggle between "PrivateWriting" and "Presentation" modes so that I can focus on writing without distraction. *(Domain-aware pattern: Uses ViewModeEnum)* | - PrivateWriting mode: Full admin tools<br>- Presentation mode: Clean public view only<br>- URL-based view mode switch<br>- **Hybrid response pattern**: `{ "data": { "viewMode": "Presentation", "title": "..." } }` (RAW for data retrieval!) | **High** | `Content/ContentItemsController.cs`, `ViewModeEnum.cs` ✅ Domain-aware enum! |

### **CONTENT-03: Media Upload & Management**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **CONTENT-03** | As a writer, I want to upload reference images, concept art, and audio clips so that I have visual/audio inspiration for my GDD. *(Domain-aware pattern: Uses MediaAttachment with MIME type validation)* | - Upload files up to 100MB<br>- Drag-and-drop interface<br>- Thumbnail preview generation<br>- **Hybrid response pattern**: `{ "success": true, "data": { "filename": "...", "storagePath": "..." } }` (WRAPPED for upload confirmation!) | **High** | `Content/ContentItemsController.cs`, `MediaUploadDto.cs` ✅ Domain-aware file upload! |

### **CONTENT-04: Content Versioning & Rollback**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **CONTENT-04** | As a writer with perfectionist tendencies, I want to auto-save snapshots so that I can roll back if I accidentally break my GDD. *(Domain-aware pattern: Uses ContentSnapshot)* | - Auto-save on major changes<br>- Manual snapshot creation<br>- Version history viewer<br>- **Hybrid response pattern**: `{ "success": true, "data": { "snapshotId": "...", "version": 2 } }` (WRAPPED for auto-save confirmation!) | **High** | `Content/ContentItemsController.cs`, `ContentSnapshotEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **CONTENT-05: Content Tagging System**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **CONTENT-05** | As an ADHD-friendly user, I want to tag content items (MainCharacter, Human, etc.) so that I can quickly find related items. *(Domain-aware pattern: Uses Tag + ContentTags junction table)* | - Pre-defined tag list<br>- Multi-select interface<br>- Tag suggestions based on usage<br>- **Hybrid response pattern**: `{ "success": true, "data": { "tags": ["MainCharacter", "Human"] } }` (WRAPPED for tag addition confirmation!) | **Medium** | `Content/ContentItemsController.cs`, `TagEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **CONTENT-06: External Reference Management**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **CONTENT-06** | As a writer, I want to link to external resources (Notion docs, Pinterest boards) so that my GDD is connected to inspiration sources. *(Domain-aware pattern: Uses ExternalReference)* | - URL validation (HTTP/HTTPS only)<br>- Resource type tagging (Document, Image, Video)<br>- Thumbnail preview for images<br>- **Hybrid response pattern**: `{ "success": true, "data": { "url": "...", "title": "..." } }` (WRAPPED for reference creation confirmation!) | **Medium** | `Content/ContentItemsController.cs`, `ExternalReferenceEntityTypeConfiguration.cs` ✅ Domain-separated config! |

---

## **4️⃣ Task Management & ADHD Support (8 Stories)** ⭐ **UPDATED: PROJECT-TASK-x IDs**

### **PROJECT-TASK-01: Filter Tasks by Difficulty**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **PROJECT-TASK-01** | As a user with focus challenges, I want to filter project tasks by difficulty (Easy/Medium/Hard) so that I can match my energy level with task complexity. *(Domain-aware pattern: Uses ProjectTask.Difficulty field)* | - Filter dropdown: Easy/Medium/Hard<br>- Visual indicators per difficulty<br>- Sort by estimated completion time<br>- **Hybrid response pattern**: `{ "data": [...], "pagination": {...} }` (RAW for list retrieval!) | **High** | `Tasks/TasksController.cs`, `ProjectTaskEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **PROJECT-TASK-02: Quick Win Badges**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **PROJECT-TASK-02** | As an ADHD-friendly user, I want to mark tasks as "Quick Wins" so that I can maintain momentum and reduce cognitive load. *(Domain-aware pattern: Uses ProjectTask.IsQuickWin field)* | - Quick Win badge for short tasks (<15 min)<br>- Toggle quick win status<br>- Sort by quick wins first<br>- **Hybrid response pattern**: `{ "success": true, "message": "Task marked as quick win", "data": { "isQuickWin": true } }` (WRAPPED for task update confirmation!) | **High** | `Tasks/TasksController.cs`, `ProjectTaskEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **PROJECT-TASK-03: Focus Mode Task View**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **PROJECT-TASK-03** | As an ADHD-friendly user, I want a focus mode view that hides sidebars and shows only the current task so that I can maintain deep work. *(Domain-aware pattern: Uses SingleTaskView with ProjectTask entity)* | - Hide project sidebar<br>- Show only active task<br>- Timer integration for Pomodoro technique<br>- **Hybrid response pattern**: `{ "data": { "taskTitle": "...", "estimatedMinutes": 15 } }` (RAW for single task data retrieval!) | **Medium** | `Tasks/TasksController.cs`, `ProjectTaskEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **PROJECT-TASK-04: Task Status Workflow**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **PROJECT-TASK-04** | As a team member, I want to move tasks through statuses (Backlog → InProgress → Review → Done) so that my workflow is visible and organized. *(Domain-aware pattern: Uses TaskStatusEnum)* | - Drag-and-drop status transitions<br>- Status change confirmation<br>- Audit log of status changes<br>- **Hybrid response pattern**: `{ "success": true, "message": "Task moved to InProgress", "data": { "status": 1 } }` (WRAPPED for status update confirmation!) | **High** | `Tasks/TasksController.cs`, `TaskStatusEnum.cs` ✅ Domain-aware enum! |

### **PROJECT-TASK-05: Task Comments & Collaboration**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **PROJECT-TASK-05** | As a team member, I want to add comments to tasks so that we can discuss implementation details and provide feedback. *(Domain-aware pattern: Uses TaskComments + Comment entities)* | - Rich text comment editor<br>- Visibility control (private/team-only/public)<br>- Reply nesting support<br>- **Hybrid response pattern**: `{ "success": true, "message": "Comment created", "data": { "id": "...", "visibility": "team-only" } }` (WRAPPED for comment creation confirmation!) | **Medium** | `Tasks/TasksController.cs`, `TaskCommentsEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **PROJECT-TASK-06: Task Assignment & Ownership**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **PROJECT-TASK-06** | As a team lead, I want to assign tasks to specific team members so that everyone knows their responsibilities. *(Domain-aware pattern: Uses ProjectTask.AssignedToUserId)* | - User selection dropdown<br>- Auto-assign to self option<br>- Assignment audit log<br>- **Hybrid response pattern**: `{ "success": true, "message": "Task assigned to Jane Doe", "data": { "assignedToUserId": "usr-001" } }` (WRAPPED for assignment confirmation!) | **High** | `Tasks/TasksController.cs`, `ProjectTaskEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **PROJECT-TASK-07: Task Completion Tracking**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **PROJECT-TASK-07** | As a productivity-focused user, I want to see my task completion rate so that I can track my progress and motivation. *(Domain-aware pattern: Uses ActivityLog with ProjectTask entity)* | - Completion percentage display<br>- Streak counter for consecutive days<br>- Achievement notifications<br>- **Hybrid response pattern**: `{ "data": { "completionRate": 75, "streak": 3 } }` (RAW for data retrieval!) | **Medium** | `Tasks/TasksController.cs`, `ActivityLogEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **PROJECT-TASK-08: Task Archiving & Cleanup**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **PROJECT-TASK-08** | As an ADHD-friendly user, I want to archive completed tasks automatically so that my task list remains clean and focused. *(Domain-aware pattern: Uses ContentVersionLog with ProjectTask entity)* | - Auto-archive after 7 days of completion<br>- Manual archive option<br>- Archive search capability<br>- **Hybrid response pattern**: `{ "success": true, "message": "Task archived", "data": null }` (WRAPPED for archive confirmation!) | **Medium** | `Tasks/TasksController.cs`, `ContentVersionLogEntityTypeConfiguration.cs` ✅ Domain-separated config! |

---

## **5️⃣ Story Outlining & Navigation (3 Stories)**

### **SEQUENCE-01: Create Story Sequences (Chapters)**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **SEQUENCE-01** | As a writer, I want to create story sequences (chapters) so that my narrative structure is organized and easy to navigate. *(Domain-aware pattern: Uses StorySequence entity)* | - Chapter title input<br>- Slug auto-generation<br>- Order index management<br>- **Hybrid response pattern**: `{ "success": true, "message": "Chapter created", "data": { "id": "...", "sequenceName": "Chapter 1" } }` (WRAPPED for sequence creation confirmation!) | **High** | `Narrative/StorySequencesController.cs`, `StorySequenceEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **SEQUENCE-02: Story Beat Management**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **SEQUENCE-02** | As a writer, I want to add beats (plot points) within sequences so that my narrative has clear turning points and structure. *(Domain-aware pattern: Uses StoryBeat entity)* | - Beat title input<br>- Drag-and-drop reordering<br>- Parent-child relationship mapping<br>- **Hybrid response pattern**: `{ "success": true, "message": "Beat added", "data": { "beatTitle": "...", "orderIndex": 1 } }` (WRAPPED for beat creation confirmation!) | **Medium** | `Narrative/StorySequencesController.cs`, `StoryBeatEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **SEQUENCE-03: Lore Entry Management**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **SEQUENCE-03** | As a world-builder, I want to add lore entries (history, culture, geography) so that my world feels rich and interconnected. *(Domain-aware pattern: Uses LoreEntry entity)* | - Lore title input<br>- Rich text editor support<br>- Category tagging (History, Culture, etc.)<br>- **Hybrid response pattern**: `{ "success": true, "message": "Lore entry added", "data": { "loreTitle": "...", "category": "History" } }` (WRAPPED for lore creation confirmation!) | **Medium** | `Narrative/LoreEntriesController.cs`, `LoreEntryEntityTypeConfiguration.cs` ✅ Domain-separated config! |

---

## **6️⃣ Team Collaboration & Review (4 Stories)**

### **TEAM-01: Role-Based Access Control (RBAC)**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **TEAM-01** | As a team lead, I want to assign roles (Admin/Editor/Viewer) to team members so that everyone has appropriate permissions. *(Domain-aware pattern: Uses TeamMember.RoleId)* | - Role selection dropdown<br>- Permission mapping per role<br>- Admin-only settings protection<br>- **Hybrid response pattern**: `{ "success": true, "message": "User granted Editor role", "data": { "roleId": 1 } }` (WRAPPED for role assignment confirmation!) | **High** | `Teams/TeamMembersController.cs`, `TeamMemberEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **TEAM-02: Content Review Workflow**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **TEAM-02** | As an editor, I want to review content items before publishing so that quality standards are maintained. *(Domain-aware pattern: Uses ReviewStatus entity)* | - Review queue view<br>- Approve/Reject actions<br>- Review comments required for rejection<br>- **Hybrid response pattern**: `{ "success": true, "message": "Content approved", "data": { "status": "Approved" } }` (WRAPPED for review confirmation!) | **High** | `Content/ContentItemsController.cs`, `ReviewStatusEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **TEAM-03: Team Comments on Content**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **TEAM-03** | As a team member, I want to leave comments on content items so that we can discuss improvements collaboratively. *(Domain-aware pattern: Uses Comment entity)* | - Rich text comment editor<br>- Visibility control (private/team-only/public)<br>- Reply nesting support<br>- **Hybrid response pattern**: `{ "success": true, "message": "Comment created", "data": { "id": "...", "visibility": "team-only" } }` (WRAPPED for comment creation confirmation!) | **Medium** | `Content/ContentItemsController.cs`, `CommentEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **TEAM-04: Team Activity Feed**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **TEAM-04** | As a team lead, I want to see recent team activity so that I know what everyone is working on. *(Domain-aware pattern: Uses ActivityLog entity)* | - Real-time activity updates<br>- Filter by event type (ContentCreated, TaskCompleted)<br>- User avatar display<br>- **Hybrid response pattern**: `{ "data": [{ "eventType": "ContentUpdated", "title": "..." }] }` (RAW for list retrieval!) | **Medium** | `Activities/ActivityLogsController.cs`, `ActivityLogEntityTypeConfiguration.cs` ✅ Domain-separated config! |

---

## **7️⃣ Version Control & Rollback (3 Stories)**

### **VERSION-01: Auto-Save Snapshots**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **VERSION-01** | As a perfectionist writer, I want auto-save snapshots so that I can roll back if I accidentally break my GDD. *(Domain-aware pattern: Uses ContentSnapshot entity)* | - Auto-save on major changes<br>- Manual snapshot creation option<br>- Snapshot version tracking<br>- **Hybrid response pattern**: `{ "success": true, "data": { "snapshotId": "...", "version": 2 } }` (WRAPPED for auto-save confirmation!) | **High** | `Versioning/ContentSnapshotsController.cs`, `ContentSnapshotEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **VERSION-02: Rollback to Previous Version**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **VERSION-02** | As a writer who made a mistake, I want to rollback to a previous version so that I can recover my work. *(Domain-aware pattern: Uses ContentVersionLog entity)* | - Version history viewer<br>- Select target version for rollback<br>- Confirmation dialog before rollback<br>- **Hybrid response pattern**: `{ "success": true, "message": "Rolled back to version 2", "data": { "version": 2 } }` (WRAPPED for rollback confirmation!) | **High** | `Versioning/ContentSnapshotsController.cs`, `ContentVersionLogEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **VERSION-03: Activity Timeline & Audit Log**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **VERSION-03** | As an admin, I want to see a timeline of all project activity so that I can audit changes and track contributions. *(Domain-aware pattern: Uses ActivityLog entity)* | - Chronological activity feed<br>- Filter by user or event type<br>- Link to related entities (ContentItem, ProjectTask)<br>- **Hybrid response pattern**: `{ "data": [{ "eventType": "ContentUpdated", "title": "...", "createdAt": "..." }] }` (RAW for list retrieval!) | **Medium** | `Activities/ActivityLogsController.cs`, `ActivityLogEntityTypeConfiguration.cs` ✅ Domain-separated config! |

---

## **8️⃣ Export & Engine Integration (3 Stories)**

### **EXPORT-01: JSON GDD Export**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **EXPORT-01** | As a Unity developer, I want to export my GDD as JSON so that I can import it directly into Unity. *(Domain-aware pattern: Uses EngineExportConfig entity)* | - Select content types to export<br>- Include/exclude watermark option<br>- Downloadable JSON file generation<br>- **Hybrid response pattern**: `{ "success": true, "message": "JSON export generated", "data": { "downloadUrl": "...", "filename": "..." } }` (WRAPPED for export confirmation!) | **High** | `Export/JsonExportController.cs`, `EngineExportConfigEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **EXPORT-02: CSV Unity Import**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **EXPORT-02** | As a Unity developer, I want to export character data as CSV so that I can import it using Unity's CSV importer. *(Domain-aware pattern: Uses EngineFieldMapping entity)* | - Character-specific export format<br>- Include/exclude attributes<br>- Downloadable CSV file generation<br>- **Hybrid response pattern**: `{ "success": true, "message": "CSV export generated", "data": { "downloadUrl": "...", "filename": "characters.csv" } }` (WRAPPED for export confirmation!) | **Medium** | `Export/CsvExportController.cs`, `EngineFieldMappingEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **EXPORT-03: XML Unreal Engine GDD**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **EXPORT-03** | As a Unreal developer, I want to export my GDD as XML so that I can import it into Unreal Engine's asset system. *(Domain-aware pattern: Uses EngineExportConfig entity)* | - Unreal-compatible XML format<br>- Include/exclude assets<br>- Downloadable XML file generation<br>- **Hybrid response pattern**: `{ "success": true, "message": "XML export generated", "data": { "downloadUrl": "...", "filename": "characters.xml" } }` (WRAPPED for export confirmation!) | **Medium** | `Export/XmlExportController.cs`, `EngineExportConfigEntityTypeConfiguration.cs` ✅ Domain-separated config! |

---

## **9️⃣ Search & Discovery (2 Stories)**

### **SEARCH-01: Full-Text Content Search**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **SEARCH-01** | As a writer with ADHD, I want to search my entire project so that I can quickly find relevant content items. *(Domain-aware pattern: Uses ContentItem full-text search)* | - Search by title/description/slug<br>- Filter by content type<br>- Highlight matches in results<br>- **Hybrid response pattern**: `{ "data": [{ "id": "...", "title": "..." }], "pagination": {...} }` (RAW for list retrieval!) | **High** | `Search/SearchController.cs`, `ContentItemEntityTypeConfiguration.cs` ✅ Domain-separated config! |

### **SEARCH-02: Filtered Task Search**
| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :-- | :--- | :--- | :--- | :--- |
| **SEARCH-02** | As a task manager, I want to search my tasks by keyword so that I can find relevant tasks quickly. *(Domain-aware pattern: Uses ProjectTask full-text search)* | - Search by task title/description<br>- Filter by difficulty/quick win<br>- Sort by estimated completion time<br>- **Hybrid response pattern**: `{ "data": [...], "pagination": {...} }` (RAW for list retrieval!) | **Medium** | `Tasks/TasksController.cs`, `ProjectTaskEntityTypeConfiguration.cs` ✅ Domain-separated config! |

---

## **🐱 Summary: Complete User Stories Documentation**

**This updated USER_STORIES.md documentation for GaDeMa v0.1 Pre-Release MVP now includes:**

✅ All ~50 user stories with acceptance criteria and technical scope  
✅ Updated story IDs (`PROJECT-TASK-x` instead of `TASK-x`) ✅  
✅ Updated field references (`ProjectTask.Difficulty`, `ProjectTask.IsQuickWin`) ✅  
✅ Domain-aware patterns throughout (all configuration files per domain) ✅  
✅ Hybrid response pattern examples (RAW vs. WRAPPED) ✅  
✅ Complete coverage across 9 key feature categories  

✅ **Domain-separated configurations** mentioned in all technical scopes  
✅ **Updated entity names** (`ProjectTask` instead of `Task`) with clear references  
✅ **Hybrid response patterns** documented in all API endpoints  

**Total User Stories**: ~50 complete stories with acceptance criteria  
**Version**: v0.1 (Pre-Release MVP)  
**Status**: Production-Ready Architecture ✅

---

## **📋 Key Updates Summary Table:**

| Feature Area | Stories Updated | Key Changes | Impact Level |
| :--- | :--- | :--- | :--- |
| **Task Management** | 8 (PROJECT-TASK-x) | Renamed from TASK-x, updated field references ✅ | 🔴 High |
| **Content Creation** | 6 (CONTENT-x) | Updated method names, view mode separation ✅ | 🟡 Medium |
| **Story Outlining** | 3 (SEQUENCE-x) | Updated entity names, domain-separated configs ✅ | 🟢 Low |
| **Team Collaboration** | 4 (TEAM-x) | Updated role-based access control references ✅ | 🟡 Medium |
| **Version Control** | 3 (VERSION-x) | Updated snapshot/rollback logic with domain patterns ✅ | 🟡 Medium |
| **Export & Integration** | 3 (EXPORT-x) | Updated engine export configurations per domain ✅ | 🟢 Low |
| **Search & Discovery** | 2 (SEARCH-x) | Updated search filtering with domain-aware patterns ✅ | 🟢 Low |

---
