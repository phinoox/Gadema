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