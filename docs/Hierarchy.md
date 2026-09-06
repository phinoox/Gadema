# 📊 **GaDeMa Hierarchical Model Overview**

## **Root Level: Project Management (1 model)**

```
┌─────────────┐
│  Project    │── Owner (User/Team)
└─────────────┘
      │
      └──> Contains all MetaInfos, TeamMemberships
```

---

## **Level 2: Authentication & Ownership**

### **User Management**
```
┌─────────────┐
│  User       │── Password Hash, Google OAuth, Recovery Codes
└─────────────┘
      │
      └──> TeamMember (many-to-many via join table)
```

### **Team Management**
```
┌─────────────┐
│  Team       │── Multi-user project collaboration
└─────────────┘
      │
      └──> TeamMember (role-based access control)
```

---

## **Level 3: Content Items (Generic Container - ~57 tables total)**

### **Character MetaInfo**
```
MetaInfo (Character)
│
├── CharacterDetails       │── FK to MetaInfo, PK as FK to CharacterAttributes
├── CharacterBackground    │── FK to MetaInfo (back-reference needed!)
├── CharacterAttributes    │── FK to MetaInfo (back-reference needed!)
├── ProjectTasks           │── Optional FK to MetaInfo (content-specific tasks)
├── MediaAttachments       │── FK to MetaInfo, cascade delete
├── ExternalReferences     │── FK to MetaInfo, maintain link history
├── Tags/ContentTags       │── Junction table for multi-select tagging
├── CharacterIdentities    │── Identity assignments (race/faction/alignment)
└── ReviewStatus           │── Approval workflow for character content
```

### **World/Locations MetaInfo**
```
MetaInfo (World/Location)
│
├── StorySequence         │── FK to MetaInfo (narrative structure within world)
├── StoryBeat             │── FK to MetaInfo (plot points within world)
├── LoreEntry             │── FK to MetaInfo (world-building lore)
├── DialogueBranch        │── FK to MetaInfo (visual novel dialogue)
├── DialogueNode          │── FK to DialogueBranch, part of tree structure
└── MediaAttachments      │── FK to MetaInfo, cascade delete
```

### **Mechanics/Systems MetaInfo**
```
MetaInfo (Mechanic/System)
│
├── ClassTemplateAttribute│── FK to MetaInfo (attribute mappings for class template)
├── AbilityDefinition     │── FK to MetaInfo (abilities within mechanic system)
└── MediaAttachments      │── FK to MetaInfo, cascade delete
```

### **General Purpose MetaInfos**
```
MetaInfo (Generic)
│
├── StoryOutline          │── FK to Project OR MetaInfo (narrative structure planning)
├── DialogueBranch        │── FK to MetaInfo (visual novel dialogue for generic content)
└── MediaAttachments      │── FK to MetaInfo, cascade delete
```

---

## **Level 4: Cross-Cutting Concerns (Apply to ALL MetaInfos)**

### **Task Management**
```
┌─────────────┐
│ ProjectTask │── Optional FK to MetaInfo (content-specific task)
└─────────────┘
      │
      └──> TaskComments (comments specific to this task)
            └──> Comment (general content comments)
```

### **Version Control**
```
┌─────────────┐
│ ContentSnapshot│── FK to MetaInfo, version tracking for rollback
└─────────────┘
```

### **Activity Monitoring**
```
┌─────────────┐
│ ActivityLog  │── Generic RelatedEntityId (can be MetaInfo or Project)
└─────────────┘
      │
      └──> TokenUsageLog (API token usage tracking for audit/monitoring)
```

### **Tagging System**
```
┌─────────────┐
│ Tag         │── Reusable tag definitions (MainCharacter, Hero, etc.)
└─────────────┘
      │
      └──> ContentTags (Junction table linking Tags to all MetaInfos)
```

### **Media Management**
```
┌─────────────┐
│ MediaAttachment │── FK to MetaInfo, cascade delete when content deleted
└─────────────┘
      │
      └──> MediaTags (Tagging for media files with category labels)
```

### **Review Workflow**
```
┌─────────────┐
│ ReviewStatus  │── FK to MetaInfo, approval/rejection workflow
└─────────────┘
```

### **Identity System**
```
┌─────────────┐
│ ProjectIdentityDefinition │── Defines available identity types per project
└─────────────┘
      │
      └──> IdentityValue (Specific values for each identity type)
            └──> CharacterIdentity (Links characters to their assigned identities)
```

### **Export Configuration**
```
┌─────────────┐
│ EngineExportConfig │── Export format config (JSON, CSV, XML, PDF)
└─────────────┘
      │
      ├──> EngineFieldMapping (Maps GaDeMa fields to engine-compatible names)
      └──> AssetLink (Cross-platform asset references between projects)
```

---

## **Level 5: Entity Relationships Summary**

### **Direct FKs to MetaInfo:**
1. ✅ CharacterDetails (strong relationship)
2. ✅ CharacterBackground (strong relationship)
3. ✅ CharacterAttributes (strong relationship)
4. ✅ StorySequence (narrative structure within content item)
5. ✅ StoryBeat (plot points within content item)
6. ✅ LoreEntry (world-building lore for content item)
7. ✅ DialogueBranch (visual novel dialogue within content item)
8. ✅ DialogueNode (node in dialogue tree for content item)
9. ✅ MediaAttachment (files attached to content item)
10. ✅ ExternalReference (external resources linked to content item)
11. ✅ ContentTag (tagging junction table)
12. ✅ CharacterIdentity (identity assignments for character content items)
13. ✅ ReviewStatus (approval workflow for content items)
14. ✅ ProjectTask (optional FK for content-specific tasks)
15. ✅ ContentSnapshot (version tracking for rollback)

### **Generic Relationships:**
- ⚠️ ActivityLog (generic `RelatedEntityId`, can link to any entity including MetaInfo)

---

## **📊 Visual Hierarchy Diagram:**

```
┌───────────────────────────────────────────────────────┐
│                    PROJECT                            │
└───────────────────────────────────────────────────────┘
                      │
        ┌─────────────┼─────────────┐
        ▼             ▼             ▼
   ┌──────────┐  ┌──────────┐  ┌──────────┐
   │ CONTENT  │  │ TEAM     │  │ USERS    │
   │ ITEM     │  │ MEMBERSHIP│  │          │
   └──────────┘  └──────────┘  └──────────┘
        │
        ├───► CHARACTER CONTENT ITEMS
        │         ├── CharacterDetails
        │         ├── CharacterBackground
        │         ├── CharacterAttributes
        │         ├── DialogueBranch/Node
        │         └── MediaAttachments
        │
        ├───► WORLD CONTENT ITEMS
        │         ├── StorySequence
        │         ├── StoryBeat
        │         ├── LoreEntry
        │         └── MediaAttachments
        │
        ├───► MECHANICS CONTENT ITEMS
        │         ├── ClassTemplateAttribute
        │         ├── AbilityDefinition
        │         └── MediaAttachments
        │
        ├───► GENERAL CONTENT ITEMS
        │         ├── StoryOutline
        │         ├── DialogueBranch/Node
        │         └── MediaAttachments
        │
        └───► CROSS-CUTTING CONCERNS (ALL MetaInfos)
                  ├── ProjectTasks (optional FK)
                  ├── ReviewStatus
                  ├── Tags/Junction Table
                  ├── Snapshots
                  ├── ActivityLog
                  └── Identity System (for Character items only)
```

---

## **📋 Navigation Property Requirements:**

**Every entity that has a `MetaInfoId` FK should also have:**
- ✅ A back-reference navigation property to `MetaInfo` (except junction tables)
- ✅ Eager loading capability via `Include()` pattern
- ✅ Proper cascade delete or restrict behavior based on relationship type

**Exception: Junction Tables**
- `ContentTags` → Has two navigation properties (one to MetaInfo, one to Tag)

---

## **🎯 Key Design Patterns:**

1. **Generic Container Pattern**: MetaInfo holds metadata while specific data lives in child entities
2. **Strong vs Optional Relationships**: Most are strong (cascade delete), ProjectTasks is optional (restrict)
3. **Cross-Cutting Concerns**: Tasks, Reviews, Tags, Snapshots apply to ALL MetaInfos uniformly
4. **Eager Loading Pattern**: All navigation properties enable efficient eager loading to prevent N+1 queries

---

This hierarchical structure ensures clean separation of concerns where `MetaInfo` acts as the generic metadata container for all game development content types, with specific data stored in child entities! 🎮✨