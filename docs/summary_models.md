Here's a comprehensive summary of all the **Models** and their **EF Core Configurations** in the Gadema project:

---

## Models & Configurations Summary

### 1. Core/Content Models

| Model | Configuration | Key Details |
|-------|--------------|-------------|
| **`Comment`** | `CommentEntityTypeConfiguration` | Links to ContentItem; tracks comments with visibility (private/team/public), max 4096 chars |
| **`ContentItem`** | `ContentItemEntityTypeConfiguration` | Core polymorphic entity; has ContentType, Status, ViewMode enums; links to Project, MediaAttachments, Comments, etc. |
| **`Tag`** | `TagEntityTypeConfiguration` | Content organization tags; unique slug; links to ContentTags & MediaTags junction tables |
| **`ContentTags`** | `ContentTagsEntityTypeConfiguration` | Junction table: ContentItem ↔ Tag |
| **`MediaTags`** | `MediaTagsEntityTypeConfiguration` | Junction table: MediaAttachment ↔ Tag |
| **`MediaAttachment`** | `MediaAttachmentEntityTypeConfiguration` | File uploads (images, PDFs); links to ContentItem; has FileName, ContentType, StoragePath, FileSize |
| **`ExternalReference`** | `ExternalReferenceEntityTypeConfiguration` | External links (Google Docs, Pinterest); self-referencing parent; unique URL index |
| **`DialogueBranch`** | `DialogueBranchEntityTypeConfiguration` | Branching narrative tree; self-referencing ParentNode; unique slug |
| **`DialogueNode`** | `DialogueNodeEntityTypeConfiguration` | Dialogue within branches; has Speaker (User), ChoiceOptions (JSON), Conditions (JSON); self-referencing ChildNodes |
| **`StoryOutline`** | `StoryOutlineEntityTypeConfiguration` | Chapter/section outlines; links to StorySequence; has CharacterSnapshot, ThemeStatement |
| **`StorySequence`** | `StorySequenceEntityTypeConfiguration` | Chapters/hierarchy; self-referencing ParentSequence; contains Outlines & Beats |
| **`StoryBeat`** | `StoryBeatEntityTypeConfiguration` | Individual story beats; links to StorySequence; unique slug |
| **`LoreEntry`** | `LoreEntryEntityTypeConfiguration` | World-building entries; LoreType enum; unique slug |

---

### 2. Authentication/Authorization Models

| Model | Configuration | Key Details |
|-------|--------------|-------------|
| **`User`** | `UserEntityTypeConfiguration` | User accounts; Google OAuth support; 2FA; unique username/email/GoogleSubjectId |
| **`Team`** | `TeamEntityTypeConfiguration` | Team org; unique slug; links to CreatedByUser (restrict) |
| **`TeamMember`** | `TeamMemberEntityTypeConfiguration` | Team membership with roles (Admin/Editor/Viewer); links to Team (cascade) & User (restrict) |
| **`UserProviderLink`** | `UserProviderLinkEntityTypeConfiguration` | Audit trail for OAuth provider links; composite PK (UserId, Provider) |

---

### 3. Project Model

| Model | Configuration | Key Details |
|-------|--------------|-------------|
| **`Project`** (in `Projects` namespace) | `ProjectEntityTypeConfiguration` | Core entity; polymorphic ownership (User/Team via OwnerType+OwnerId); has Series tracking; Visibility, Status enums |

---

### 4. Character Models

| Model | Configuration | Key Details |
|-------|--------------|-------------|
| **`CharacterDetails`** | `CharacterDetailsEntityTypeConfiguration` | Uses FK-as-PK pattern with ContentItem; has Name, Level, ClassTemplateId, Role, Status |
| **`CharacterBackground`** | `CharacterBackgroundEntityTypeConfiguration` | Uses FK-as-PK pattern; links to CharacterDetails; has Description, Published flag |

---

### 5. Abilities/Attributes Models (GAS-like architecture)

| Model | Configuration | Key Details |
|-------|--------------|-------------|
| **`AttributeDefinition`** | `AttributeDefinitionEntityTypeConfiguration` | Attribute stats (Health, AttackPower); has ValueType, FormulaExpression, LevelMappingJson; unique slug |
| **`AttributeSet`** | `AttributeSetEntityTypeConfiguration` | Groups attributes; links to Project; has Name, DisplayOrder, IsActive |
| **`ClassTemplate`** | `ClassTemplateEntityTypeConfiguration` | Character class configs (Warrior, Mage); links to AttributeSet; has BaseLevel, MaxLevel |
| **`ClassTemplateAttribute`** | `ClassTemplateAttributeEntityTypeConfiguration` | Per-class attribute overrides; composite PK (ClassTemplateId, AttributeDefinitionId) |
| **`CharacterAttributes`** | `CharacterAttributesEntityTypeConfiguration` | Stored attribute values per character; composite PK (ContentItemId, AttributeDefinitionId) |
| **`AbilityDefinition`** | `AbilityDefinitionEntityTypeConfiguration` | Individual abilities (Fireball, Heal); has AbilityType enum, CooldownSeconds, ResourceCost, ScalingFormulaJson |
| **`AbilitySet`** | `AbilitySetEntityTypeConfiguration` | Groups abilities; links to Project; has Type enum (Combat/Non-Combat/Hybrid); unique slug |
| **`StatusEffectDefinition`** | `StatusEffectDefinitionEntityTypeConfiguration` | Buffs/debuffs; has EffectType enum, DurationSeconds, DamagePerTick; unique slug |

---

### 6. Identity Models

| Model | Configuration | Key Details |
|-------|--------------|-------------|
| **`IdentityDefinition`** | *(no config found)* | Defines identity types (Race, Faction, Alignment, Guild) per project |
| **`ProjectIdentityDefinition`** | `ProjectIdentityDefinitionEntityTypeConfiguration` | Project-scoped identity; unique index on (ProjectId, IdentityName); links to Project (cascade) |
| **`IdentityValue`** | `IdentityValueEntityTypeConfiguration` | Specific selectable values (Human, Elf, Orc); links to IdentityDefinition & Project; unique slug |
| **`CharacterIdentity`** | `CharacterIdentityEntityTypeConfiguration` | Character's identity assignments; links to IdentityDefinition & IdentityValue; has IsPrimary flag |

---

### 7. Inventory Models

| Model | Configuration | Key Details |
|-------|--------------|-------------|
| **`InventoryItem`** | `InventoryItemEntityTypeConfiguration` | Game assets (collectibles, keys, achievements); ItemType enum; links to Project (cascade) |
| **`EndingDefinition`** | `EndingDefinitionEntityTypeConfiguration` | Branching narrative endings; unique slug; links to Project (cascade); has ConditionsJson |

---

### 8. Template Models

| Model | Configuration | Key Details |
|-------|--------------|-------------|
| **`ProjectTemplate`** | `ProjectTemplateEntityTypeConfiguration` | Quick project creation templates (FantasyBook, ActionRPG, SciFi); unique TemplateType index |
| **`TemplateNarrativeStructure`** | `TemplateNarrativeStructureEntityTypeConfiguration` | Pre-configured story arcs; composite PK (ProjectTemplateId, SequenceName) |
| **`TemplateClassTemplateDefinition`** | `TemplateClassTemplateDefinitionEntityTypeConfiguration` | Class templates for project templates; composite PK (ProjectTemplateId, ClassTemplateName) |
| **`TemplateIdentityDefinition`** | `TemplateIdentityDefinitionEntityTypeConfiguration` | Identity system definitions for templates; composite PK (ProjectTemplateId, IdentityName) |
| **`AttributeSetDefinition`** | *(no config found)* | Attribute set definitions for project templates |
| **`TemplateAttributeSetDefinition`** | `TemplateAttributeSetDefinitionEntityTypeConfiguration` | Links template to attribute set definition; composite PK (ProjectTemplateId, Name) |

---

### 9. Engine Integration Models

| Model | Configuration | Key Details |
|-------|--------------|-------------|
| **`EngineExportConfig`** | `EngineExportConfigEntityTypeConfiguration` | Engine-specific export configs (Unity/Unreal/Both); has ExportFormat enum; links to Project (cascade) |
| **`EngineFieldMapping`** | `EngineFieldMappingEntityTypeConfiguration` | Maps content fields to engine property names; composite PK (EngineExportConfigId, SourceColumn, TargetColumn) |
| **`AssetLink`** | `AssetLinkEntityTypeConfiguration` | Links content items to engine assets; has EnginePath, EngineAssetId, EngineFileType; links to ContentItem (cascade) |

---

### 10. Token/Auth Models

| Model | Configuration | Key Details |
|-------|--------------|-------------|
| **`ProjectToken`** | `ProjectTokenEntityTypeConfiguration` | API tokens for projects; hashed token storage; links to Project (cascade) |
| **`TokenUsageLog`** | `TokenUsageLogEntityTypeConfiguration` (Tokens) | API usage tracking; links to ProjectToken (cascade), ContentItem (set null), Project (cascade) |

---

### 11. Versioning Models

| Model | Configuration | Key Details |
|-------|--------------|-------------|
| **`ContentSnapshot`** | `ContentSnapshotEntityTypeConfiguration` | Content snapshots for rollback; has SnapshotType enum (AutoGenerated/ManualSave/RollbackPoint); max 50000 chars JSON |
| **`ContentVersionLog`** | `ContentVersionLogEntityTypeConfiguration` | Version history; links to ContentItem (set null); tracks ChangedByUserId, ChangeDescription |

---

### 12. Task Management Models

| Model | Configuration | Key Details |
|-------|--------------|-------------|
| **`ProjectTask`** | `ProjectTaskEntityTypeConfiguration` | ADHD-friendly task management; has Status, Priority, Difficulty enums; IsQuickWin flag; links to Project & ContentItem |
| **`ProjectTaskComments`** | `ProjectTaskCommentsEntityTypeConfiguration` | Task comments; links to ProjectTask (cascade) |
| **`TaskCommentEntityTypeConfiguration`** | `TaskCommentEntityTypeConfiguration` | **DUPLICATE** config for same entity (ProjectTaskComments) |
| **`ReviewStatus`** | `ReviewStatusEntityTypeConfiguration` | Content review status (Pending/Approved/Rejected); links to ContentItem & Reviewer (restrict) |

---

### 13. Activity Tracking Models

| Model | Configuration | Key Details |
|-------|--------------|-------------|
| **`ActivityLog`** | `ActivityLogEntityTypeConfiguration` | Project event tracking; has EventType, RelatedEntityType enums; links to Project (cascade) & User (restrict) |
| **`TokenUsageLog`** | `TokenUsageLogEntityTypeConfiguration` (Activities) | **DUPLICATE** config for same entity (TokenUsageLog) |

---

## Key Observations

1. **Duplicate Configurations**: `TokenUsageLog` and `ProjectTaskComments` each have duplicate configuration files in different folders (`Tokens/` and `Activities/` for TokenUsageLog; `Tasks/` has both `ProjectTaskCommentsEntityTypeConfiguration.cs` and `TaskCommentEntityTypeConfiguration.cs`)

2. **Models Without Configurations**: `IdentityDefinition`, `AttributeSetDefinition`, and `Comment` (Comment has config but it's in Tasks folder)

3. **FK-as-PK Pattern**: `CharacterDetails`, `CharacterBackground`, and `ContentItem` use FK-as-PK patterns for polymorphic/detail table relationships

4. **Polymorphic Ownership**: `Project` uses OwnerType + OwnerId for polymorphic ownership (User or Team)

5. **Self-Referencing Relationships**: `DialogueBranch`, `DialogueNode`, `StorySequence`, and `ExternalReference` all have self-referencing parent/child relationships

Based on the detailed review of the configuration files and the model definitions, here is the analysis of the remaining items flagged in `docs/ModelAnalysis.md`.

### 🔴 Critical Issues (Confirmed)

**1. Double-Configured Relationships (Both Sides Declare `HasForeignKey`)**
All 5 cases are confirmed in the current files. EF Core will throw an `InvalidOperationException` at runtime for these.

| Relationship | Conflict Details |
| :--- | :--- |
| **`Comment` ↔ `ContentItem`** | `CommentEntityTypeConfiguration` declares `.HasForeignKey(c => c.ContentItemId)`. `ContentItemEntityTypeConfiguration` also declares `.WithMany(ci => ci.Comments).HasForeignKey(c => c.ContentItemId)`. |
| **`Project` ↔ `ContentItem`** | `ProjectEntityTypeConfiguration` declares `.HasForeignKey(ci => ci.ProjectId)`. `ContentItemEntityTypeConfiguration` also declares `.HasForeignKey(ci => ci.ProjectId)`. |
| **`ContentItem` ↔ `MediaAttachment`** | `ContentItemEntityTypeConfiguration` declares `.WithMany(ci => ci.MediaAttachments).HasForeignKey(m => m.ContentItemId)`. `MediaAttachmentEntityTypeConfiguration` also declares `.HasForeignKey(m => m.ContentItemId)`. |
| **`ContentItem` ↔ `ReviewStatus`** | `ContentItemEntityTypeConfiguration` sets `SetNull`. `ReviewStatusEntityTypeConfiguration` sets `Restrict`. **Both double-config AND conflicting delete behaviors.** |
| **`StorySequence` (Self-Ref)** | `StorySequenceEntityTypeConfiguration` configures **both** `ParentSequence → ChildSequences` and `ChildSequences → ParentSequence` with `HasForeignKey` on both sides. |

**2. FK Property Name Mismatch**
| Relationship | Issue |
| :--- | :--- |
| **`ProjectTask` → `ProjectTaskComments`** | `ProjectTaskEntityTypeConfiguration` uses `HasForeignKey(pct => pct.ProjectTaskId)`. `ProjectTaskCommentsEntityTypeConfiguration` uses `HasForeignKey(pct => pct.TaskId)`. The model `ProjectTaskComments` **only has `ProjectTaskId`**, not `TaskId`. This will cause a runtime exception. |

**3. Wrong FK Property in Navigation**
| Relationship | Issue |
| :--- | :--- |
| **`Project` → `TeamMember`** | The `Project` model has a `TeamMembers` collection navigation, but `TeamMember` has `TeamId`, not `ProjectId`. `ProjectEntityTypeConfiguration` does not explicitly map this, so EF Core will try to infer it by convention, fail to find `ProjectId`, and likely create a shadow column or throw. |

**4. Duplicate Entity Type Configurations**
| Entity | Problem |
| :--- | :--- |
| **`TokenUsageLog`** | Configured in **both** `Tokens/TokenUsageLogEntityTypeConfiguration.cs` and `Activities/TokenUsageLogEntityTypeConfiguration.cs`. |
| **`ProjectTaskComments`** | Configured in **both** `Tasks/ProjectTaskCommentsEntityTypeConfiguration.cs` and `Tasks/TaskCommentEntityTypeConfiguration.cs`. |

**5. CharacterBackground FK Mapping Bug**
| Issue | Details |
| :--- | :--- |
| `CharacterBackgroundEntityTypeConfiguration` | Sets `.HasForeignKey(e => e.ContentItemId)` for the navigation to `CharacterDetails`. But `ContentItemId` is the **PK** of `CharacterBackground`, not the FK. The FK property is `CharacterDetailsId`. This will cause a mapping error. |

---

### 🟡 Warnings — Missing Configurations & Design Concerns (Confirmed)

**6. FK Relationships Not Explicitly Configured (Relying on Convention)**
These are confirmed missing from their respective configuration files:

| Entity | Missing FK Config | Status |
| :--- | :--- | :--- |
| `DialogueNode` | `SpeakerId → User` | Only indexed, not mapped as FK. |
| `IdentityValue` | `IdentityDefinitionId → IdentityDefinition` | Not mapped. |
| `IdentityValue` | `ProjectTemplateId → ProjectTemplate` | Not mapped. |
| `EngineFieldMapping` | `ProjectId → Project` | Not mapped. |
| `TemplateIdentityDefinition` | `IdentityDefinitionId → IdentityDefinition` | Not mapped. |
| `TemplateAttributeSetDefinition` | `AttributeSetDefinitionId → AttributeSetDefinition` | Not mapped. |
| `Comment` | `CommentedByUserId → User` | Only indexed, not mapped as FK. |

**7. Implicit / Potentially Incorrect Many-to-Many**
| Entity / Relationship | Issue |
| :--- | :--- |
| `Project.Tags` | `Project` model has `ICollection<Tag> Tags`. `Tag` has no `ProjectId`. No config exists. EF Core will create a **shadow junction table** `ProjectTag` at runtime. |
| `Project.MediaAttachments` | `Project` model has `ICollection<MediaAttachment> MediaAttachments`. `MediaAttachment` has `ContentItemId`, not `ProjectId`. No config exists. EF Core will create a **shadow junction table** `ProjectMediaAttachment` at runtime. |

---

### Summary of Required Fixes (Remaining)

| Priority | Action |
| :--- | :--- |
| 🔴 | Remove double `HasForeignKey()` from `Comment↔ContentItem`, `Project↔ContentItem`, `ContentItem↔MediaAttachment`, `ContentItem↔ReviewStatus`, and `StorySequence` self-ref. |
| 🔴 | Delete duplicate `TokenUsageLogEntityTypeConfiguration` (keep one). |
| 🔴 | Delete duplicate `ProjectTaskComments` / `TaskComment` config (keep one). |
| 🔴 | Fix `ProjectTaskComments` FK property name: change `TaskId` to `ProjectTaskId` in `ProjectTaskCommentsEntityTypeConfiguration`. |
| 🔴 | Fix `CharacterBackgroundEntityTypeConfiguration`: change `.HasForeignKey(e => e.ContentItemId)` to `.HasForeignKey(e => e.CharacterDetailsId)`. |
| 🟡 | Add explicit FK config for: `DialogueNode.SpeakerId`, `IdentityValue.IdentityDefinitionId`, `IdentityValue.ProjectTemplateId`, `EngineFieldMapping.ProjectId`, `TemplateIdentityDefinition.IdentityDefinitionId`, `TemplateAttributeSetDefinition.AttributeSetDefinitionId`, `Comment.CommentedByUserId`. |
| 🟡 | Fix `Project.Tags` implicit M2M — add junction entity or `ProjectId` to `Tag`. |
| 🟡 | Fix `Project.MediaAttachments` implicit M2M — remove from `Project` model or add `ProjectId` to `MediaAttachment`. |