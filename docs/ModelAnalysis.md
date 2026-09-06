# Database Model Foreign Key Analysis

Comprehensive analysis of foreign key relationships across all EF Core entities in the project.
Consolidated from three analysis passes, deduplicated and grouped by severity.

---

## Legend

| Severity | Label |
|----------|-------|
| 🔴 | **Critical** — will cause a runtime or build-time EF Core exception |
| 🟡 | **Warning** — missing configuration, implicit behavior, or design concern |
| 🔵 | **Info** — convention-based mapping, OK as-is but could be more explicit |

---

## 🔴 Critical Issues

### 1. Double-Configured Relationships (Both Sides Declare `HasForeignKey`)

EF Core throws `InvalidOperationException` when both sides of a relationship configure the FK with `HasForeignKey`. Only the **dependent (FK-owner) side** should declare the FK.

| Relationship | Entity A (config) | Entity B (config) | Fix |
|---|---|---|---|
| **Comment ↔ MetaInfo** | `CommentEntityTypeConfiguration`: `WithOne(c => c.MetaInfo).HasForeignKey(...)` | `MetaInfoEntityTypeConfiguration`: `WithMany(ci => ci.Comments).HasForeignKey(...)` | Remove `.HasForeignKey()` from one side (preferably the principal side) |
| **Project ↔ MetaInfo** | `ProjectEntityTypeConfiguration`: `WithMany(p => p.MetaInfos).HasForeignKey(ci => ci.ProjectId)` | `MetaInfoEntityTypeConfiguration`: `WithMany(p => p.MetaInfos).HasForeignKey(ci => ci.ProjectId)` | Remove the config from `ProjectEntityTypeConfiguration` (keep it on `MetaInfoEntityTypeConfiguration`) |
| **MetaInfo ↔ MediaAttachment** | `MetaInfoEntityTypeConfiguration`: `WithMany(ci => ci.MediaAttachments).HasForeignKey(m => m.MetaInfoId)` | `MediaAttachmentEntityTypeConfiguration`: `WithMany(ci => ci.MediaAttachments).HasForeignKey(m => m.MetaInfoId)` | Remove the config from `MetaInfoEntityTypeConfiguration` |
| **MetaInfo ↔ ReviewStatus** | `MetaInfoEntityTypeConfiguration`: `SetNull` | `ReviewStatusEntityTypeConfiguration`: `Restrict` | **Both double-config AND conflicting delete behaviors.** Unify on one side with the intended behavior. |
| **StorySequence ↔ StorySequence (self-ref)** | `StorySequenceEntityTypeConfiguration` configures **both** `ParentSequence → ChildSequences` and `ChildSequences → ParentSequence` with `HasForeignKey` on both sides. | Same file, both directions | Keep only the dependent side (`ParentSequence`) with `.HasForeignKey()`. Remove the inverse config. |

### 2. FK Property Name Mismatch

| Relationship | Issue | Fix |
|---|---|---|
| **ProjectTask → ProjectTaskComments** | `ProjectTaskEntityTypeConfiguration` uses `HasForeignKey(pct => pct.ProjectTaskId)`, but `ProjectTaskCommentsEntityTypeConfiguration` uses `HasForeignKey(pct => pct.TaskId)`. The model property is `ProjectTaskId`; `TaskId` may be a stale/alias property. | Ensure both sides reference the same FK property name (`ProjectTaskId`). |

### 3. Wrong FK Property in Navigation

| Relationship | Issue | Fix |
|---|---|---|
| **Project → TeamMember** | `ProjectEntityTypeConfiguration` maps `Project.TeamMembers` using `.HasForeignKey(tm => tm.TeamId)`. `TeamMember` has **no** `ProjectId` — it links to `Team`, not `Project`. | Either remove `Project.TeamMembers` / its config (the link is via `Team.TeamMembers`), or add a `ProjectId` FK to `TeamMember`. |

### 4. Duplicate Entity Type Configurations

| Entity | Problem | Fix |
|---|---|---|
| **TokenUsageLog** | Configured in **both** `Tokens/TokenUsageLogEntityTypeConfiguration.cs` and `Activities/TokenUsageLogEntityTypeConfiguration.cs`. EF Core will throw if both are registered. | Delete the duplicate. Keep whichever has the complete configuration. |
| **ProjectTaskComments** | Configured in both `ProjectTaskCommentsEntityTypeConfiguration` and `TaskCommentEntityTypeConfiguration`. | Merge into a single configuration file. |
| **Comment** | Configured in both `CommentEntityTypeConfiguration` and inside `MetaInfoEntityTypeConfiguration` (`.WithMany(ci => ci.Comments)`). | Keep FK config on the dependent entity (`CommentEntityTypeConfiguration`). The principal-side config should not re-declare the FK. |

### 5. CharacterBackground FK Mapping Bug

| Issue | Details |
|---|---|
| `CharacterBackgroundEntityTypeConfiguration` sets `.HasForeignKey(e => e.MetaInfoId)` with navigation `cb => cb.CharacterDetails`. But `MetaInfoId` is the **PK** of `CharacterBackground`, not the FK to `CharacterDetails`. The FK property is `CharacterDetailsId`. | Change to `.HasForeignKey(e => e.CharacterDetailsId)` or remove the redundant config if EF convention handles it. |

---

## 🟡 Warnings — Missing Configurations & Design Concerns

### 6. FK Relationships Not Explicitly Configured (Relying on Convention)

These are likely **working** via EF Core conventions, but explicit configuration is recommended for clarity and to avoid accidental breakage.

| Entity | Missing FK Config | Notes |
|---|---|---|
| `DialogueNode` | `SpeakerId → User` | Only indexed, not configured as FK. |
| `IdentityValue` | `IdentityDefinitionId → IdentityDefinition` | |
| `IdentityValue` | `ProjectTemplateId → ProjectTemplate` | |
| `EngineFieldMapping` | `ProjectId → Project` | |
| `TemplateIdentityDefinition` | `IdentityDefinitionId → IdentityDefinition` | |
| `TemplateAttributeSetDefinition` | `AttributeSetDefinitionId → AttributeSetDefinition` | |
| `Comment` | `CommentedByUserId → User` | FK column exists but is not mapped to the `User` navigation. |

### 7. Implicit / Potentially Incorrect Many-to-Many

| Entity / Relationship | Issue | Recommendation |
|---|---|---|
| `Project.Tags` | `ProjectEntityTypeConfiguration` declares `.HasMany(p => p.Tags).WithMany()` but `Tag` has **no** `ProjectId` FK and no junction table. EF Core will create a **shadow** junction table at runtime, which may not match domain intent. | Either add a `ProjectTag` junction entity with explicit config, or add `ProjectId` to `Tag` and configure as one-to-many. |
| `Project.MediaAttachments` | `ProjectEntityTypeConfiguration` declares `Project.MediaAttachments` but `MediaAttachment` has `MetaInfoId`, **not** `ProjectId`. This mapping is invalid. | Remove `Project.MediaAttachments` — media attachments belong to `MetaInfo`, not `Project`. |

---

## 🔵 Info — Convention-Based Mappings (OK as-is)

The following are mapped via EF Core conventions. They are functional but could benefit from explicit configuration for maintainability.

### Junction / Link Tables (Composite PK, FKs inferred)

| Entity | FKs (convention-mapped) |
|---|---|
| `ContentTags` | `MetaInfoId → MetaInfo`, `TagId → Tag` |
| `MediaTags` | `MediaAttachmentId → MediaAttachment`, `TagId → Tag` |
| `ClassTemplateAttribute` | `ClassTemplateId → ClassTemplate`, `AttributeDefinitionId → AttributeDefinition` |
| `CharacterAttributes` | (`MetaInfoId`, `AttributeDefinitionId`) — composite PK |
| `EngineFieldMapping` | (`EngineExportConfigId`, `SourceColumn`, `TargetColumn`) — composite PK |
| `TemplateIdentityDefinition` | (`ProjectTemplateId`, `IdentityName`) — composite PK |
| `TemplateClassTemplateDefinition` | (`ProjectTemplateId`, `ClassTemplateName`) — composite PK |
| `TemplateAttributeSetDefinition` | (`ProjectTemplateId`, `Name`) — composite PK |
| `TemplateNarrativeStructure` | (`ProjectTemplateId`, `SequenceName`) — composite PK |

### Explicitly Configured — Verified OK

| Entity | FK | Target | Delete Behavior |
|---|---|---|---|
| `MetaInfo` | `ProjectId` | `Project` | Cascade |
| `MetaInfo` | `MetaInfoId` → `MediaAttachment` | `MediaAttachment` | SetNull |
| `MetaInfo` | `MetaInfoId` → `ContentTags` | `ContentTags` | SetNull |
| `MediaAttachment` | `MetaInfoId` | `MetaInfo` | SetNull |
| `ExternalReference` | `ParentId` | `ExternalReference` (self) | Cascade |
| `DialogueBranch` | `ParentNodeId` | `DialogueBranch` (self) | Restrict |
| `DialogueNode` | `BranchId` | `DialogueBranch` | Cascade |
| `StoryOutline` | `SequenceId` | `StorySequence` | Cascade |
| `StorySequence` | `ParentSequenceId` | `StorySequence` (self) | Restrict |
| `StoryBeat` | `SequenceId` | `StorySequence` | Cascade |
| `LoreEntry` | `ProjectId` | `Project` | Cascade |
| `Team` | `CreatedByUserId` | `User` | Restrict |
| `TeamMember` | `TeamId` | `Team` | Cascade |
| `TeamMember` | `UserId` | `User` | Restrict |
| `UserProviderLink` | `UserId` | `User` | Cascade |
| `Project` | `SeriesProjectId` | `Project` (self) | Restrict |
| `Project` | `OwnerId` | `User` | Restrict |
| `CharacterDetails` | `MetaInfoId` | `MetaInfo` | Cascade |
| `ClassTemplate` | `AttributeSetId` | `AttributeSet` | Cascade |
| `AbilitySet` | `ProjectId` | `Project` | Cascade |
| `ProjectIdentityDefinition` | `ProjectId` | `Project` | Cascade |
| `IdentityValue` | `ProjectId` | `Project` | Cascade |
| `CharacterIdentity` | `IdentityDefinitionId` | `IdentityDefinition` | Restrict |
| `CharacterIdentity` | `IdentityValueId` | `IdentityValue` | SetNull |
| `InventoryItem` | `ProjectId` | `Project` | Cascade |
| `EndingDefinition` | `ProjectId` | `Project` | Cascade |
| `EngineExportConfig` | `ProjectId` | `Project` | Cascade |
| `EngineFieldMapping` | `EngineExportConfigId` | `EngineExportConfig` | (via composite PK) |
| `AssetLink` | `MetaInfoId` | `MetaInfo` | Cascade |
| `ProjectToken` | `ProjectId` | `Project` | Cascade |
| `TokenUsageLog` | `TokenId` | `ProjectToken` | Cascade |
| `TokenUsageLog` | `ContentId` | `MetaInfo` | SetNull |
| `TokenUsageLog` | `ProjectId` | `Project` | Cascade |
| `ContentVersionLog` | `MetaInfoId` | `MetaInfo` | SetNull |
| `ProjectTask` | `ProjectTaskId` | `ProjectTask` (self, PK-as-FK) | Cascade |
| `ReviewStatus` | `ReviewedByUserId` | `User` | Restrict |

### Design Observations (No Action Required)

| Observation | Details |
|---|---|
| **Polymorphic Owner** | `Project.Owner` uses `OwnerId` + `OwnerType` (int enum). EF Core maps it as a standard FK to `User`, but at runtime `OwnerType == Team` means the FK points to a `Team` ID. This is an application-layer concern, not an EF mapping error. |
| **ExternalReference.ParentType** | `ParentType` is an int enum discriminator, not a true FK column. EF Core does not map it as a relationship — it is application logic. |
| **Project.CreatedByUserId** | Plain FK column, no navigation property. Treated as a scalar, not a relationship. |
| **Junction tables without FK configs** | `ContentTags`, `MediaTags`, `ClassTemplateAttribute`, etc. rely on convention. Functional but consider adding explicit `.HasForeignKey()` for clarity. |

---

## Summary of Required Fixes

| # | Priority | Action |
|---|---|---|
| 1 | 🔴 | Remove double `HasForeignKey()` from all 5 double-configured relationships (Comment↔MetaInfo, Project↔MetaInfo, MetaInfo↔MediaAttachment, MetaInfo↔ReviewStatus, StorySequence self-ref) |
| 2 | 🔴 | Delete duplicate `TokenUsageLogEntityTypeConfiguration` (keep one) |
| 3 | 🔴 | Delete duplicate `ProjectTaskComments` / `TaskComment` config (keep one) |
| 4 | 🔴 | Fix `ProjectTaskComments` FK property name mismatch (`ProjectTaskId` vs `TaskId`) |
| 5 | 🔴 | Fix `CharacterBackgroundEntityTypeConfiguration` FK to use `CharacterDetailsId` not `MetaInfoId` |
| 6 | 🔴 | Remove `Project.TeamMembers` FK config (wrong entity — `TeamMember` links to `Team`, not `Project`) |
| 7 | 🟡 | Add explicit FK config for: `DialogueNode.SpeakerId`, `IdentityValue.IdentityDefinitionId`, `IdentityValue.ProjectTemplateId`, `EngineFieldMapping.ProjectId`, `TemplateIdentityDefinition.IdentityDefinitionId`, `TemplateAttributeSetDefinition.AttributeSetDefinitionId`, `Comment.CommentedByUserId` |
| 8 | 🟡 | Fix `Project.Tags` implicit M2M — add junction entity or `ProjectId` to `Tag` |
| 9 | 🟡 | Remove `Project.MediaAttachments` mapping (invalid — `MediaAttachment` has no `ProjectId`) |
