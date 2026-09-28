# 📋 User Stories & Acceptance Criteria

This guide provides a detailed breakdown of the user stories, acceptance criteria, and technical scope required for Anima Lab. These stories drive the development of our ADHD-friendly, non-linear content management system.

## 1. Task Management & Workflow (ADHD-Friendly) ⭐

These stories focus on reducing cognitive load through energy-matching and momentum building.

| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :--- | :--- | :--- | :--- | :--- |
| **PROJECT-TASK-01** | Filter tasks by difficulty (Easy/Medium/Hard) to match energy levels. | - Dropdown filter for difficulty<br>- Visual indicators per level<br>- Sort by estimated time | **High** | `ProjectTask` entity, Difficulty field |
| **PROJECT-TASK-02** | Mark tasks as "Quick Wins" to build momentum. | - Quick Win badge for <15m tasks<br>- Toggle status<br>- Sort by quick wins first | **High** | `IsQuickWin` boolean flag |
| **PROJECT-TASK-03** | Use a Focus Mode view that hides distractions. | - Hide sidebars/UI clutter<br>- Show only active task<br>- Timer integration | **Medium** | Single Task View UI pattern |
| **PROJECT-TASK-04** | Move tasks through statuses (Backlog $\rightarrow$ Done). | - Drag-and-drop transitions<br>- Status change audit logs<br>- Workflow visualization | **High** | `TaskStatusEnum` implementation |
| **PROJECT-TASK-05** | Add comments to tasks for collaboration. | - Rich text editor<br>- Visibility control (Private/Team)<br>- Threaded replies | **Medium** | `TaskComments` & `Comment` entities |
| **PROJECT-TASK-06** | Assign tasks to specific team members. | - User selection dropdown<br>- Assignment audit trail<br>- Auto-assign option | **High** | `AssignedToUserId` FK on `ProjectTask` |
| **PROJECT-TASK-07** | Track task completion rates and streaks. | - Completion percentage display<br>- Consecutive day streak counter | **Medium** | Activity log & Task history analysis |
| **PROJECT-TASK-08** | Automatically archive completed tasks to reduce clutter. | - Auto-archive after 7 days<br>- Manual archive option<br>- Archive search capability | **Medium** | `ContentVersionLog` / Cleanup service |

---

## 2. Content Creation & Management

| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :--- | :--- | :--- | :--- | :--- |
| **CONTENT-01** | Create content items with dedicated outlines. | - Outline creation before/during content<br>- Markdown/HTML support | **High** | `MetaInfo` + `StoryOutline` |
| **CONTENT-02** | Toggle between "PrivateWriting" and "Presentation" modes. | - Private: Full editor tools<br>- Presentation: Clean, read-only view | **High** | `ViewModeEnum` logic |
| **CONTENT-03** | Upload media (images/audio) for inspiration. | - Drag-and-drop interface<br>- Up to 100MB file size<br>- MIME type validation | **High** | `MediaAttachment` system |
| **CONTENT-04** | Auto-save snapshots and rollback versions. | - Automatic snapshot on major changes<br>- Manual version creation<br>- Rollback capability | **High** | `ContentSnapshot` & Versioning logic |
| **CONTENT-05** | Tag content for quick discovery. | - Multi-select interface<br>- Pre-defined tag lists<br>- Tag suggestions | **Medium** | `Tag` + `ContentTags` junction |
| **CONTENT-06** | Link to external resources (Notion, Pinterest). | - URL validation<br>- Resource type tagging<br>- Thumbnail previews | **Medium** | `ExternalReference` entity |

---

## 3. Narrative Structure & Outlining

| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :--- | :--- | :--- | :--- | :--- |
| **SEQUENCE-01** | Create story sequences (chapters/acts). | - Chapter title input<br>- Slug auto-generation<br>- Order index management | **High** | `StorySequence` entity |
| **SEQUENCE-02** | Add beats (plot points) within sequences. | - Beat title & description<br>- Drag-and-drop reordering<br>- Parent-child mapping | **Medium** | `StoryBeat` entity |
| **SEQUENCE-03** | Manage lore entries for world building. | - Lore category tagging<br>- Rich text editor<br>- Hierarchy support | **Medium** | `LoreEntry` entity |

---

## 4. Team Collaboration & Review

| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :--- | :--- | :--- | :--- | :--- |
| **TEAM-01** | Assign roles (Admin/Editor/Viewer) to members. | - Role selection dropdown<br>- Permission enforcement per role | **High** | `TeamMemberRoleEnum` & RBAC |
| **TEAM-02** | Review content before publishing. | - Approval/Rejection workflow<br>- Mandatory review comments on rejection | **High** | `ReviewStatus` entity |
| **TEAM-03** | Leave collaborative comments on content. | - Threaded discussions<br>- Visibility control (Team vs Public) | **Medium** | `Comment` entity |
| **TEAM-04** | View a team activity feed. | - Real-time activity updates<br>- Filter by event type | **Medium** | `ActivityLog` entity |

---

## 5. Project Setup & Configuration

| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :--- | :--- | :--- | :--- | :--- |
| **PROJ-01** | Create project from pre-defined templates. | - Template selection<br>- Auto-population of structure | **High** | `ProjectTemplate` system |
| **PROJ-02** | Organize projects into series. | - Parent/Child relationship<br>- Series name tracking | **Medium** | `SeriesId` FK on `Project` |
| **PROJ-03** | Control project visibility (Private/Public). | - Private/Public toggle<br>- Role-based access control | **High** | `ProjectVisibilityEnum` |
| **PROJ-04** | Enable/Disable user self-registration. | - Admin toggle for registration<br>- Feature flag implementation | **Medium** | Project feature flags |

---

## 6. Export & Engine Integration

| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :--- | :--- | :--- | :--- | :--- |
| **EXPORT-01** | Export GDD as JSON for Unity. | - Selectable content types<br>- Downloadable JSON file | **High** | `EngineExportConfig` (JSON) |
| **EXPORT-02** | Export character data as CSV for Unity. | - Character attribute mapping<br>- Downloadable CSV file | **Medium** | `EngineFieldMapping` (CSV) |
| **EXPORT-03** | Export XML for Unreal Engine. | - Unreal-compatible XML structure<br>- Asset field mapping | **Medium** | `EngineExportConfig` (XML) |

---

## 7. Search & Discovery

| ID | User Story | Acceptance Criteria | Priority | Technical Scope |
| :--- | :--- | :--- | :--- | :--- |
| **SEARCH-01** | Full-text search across all content. | - Title/Description/Slug search<br>- Content type filtering | **High** | `MetaInfo` full-text indexing |
| **SEARCH-02** | Filtered task search. | - Keyword search in tasks<br>- Difficulty/Status filtering | **Medium** | `ProjectTask` indexing |

***
*Last Updated: [Date]*
